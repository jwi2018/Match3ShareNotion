using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

public static class Plist
{
    private static readonly List<int> offsetTable = new List<int>();
    private static List<byte> objectTable = new List<byte>();
    private static int refCount;
    private static int objRefSize;
    private static int offsetByteSize;
    private static long offsetTableOffset;

    #region Public Functions

    public static Dictionary<string, object> createDictionaryFromXmlFile(string path)
    {
        var doc = new XmlDocument();
        doc.LoadXml(path);
        var rootNode = doc.DocumentElement.ChildNodes[0];
        return (Dictionary<string, object>) parse(rootNode);
    }

    public static Dictionary<string, object> createDictionaryFromXml(string xml)
    {
        var doc = new XmlDocument();
        doc.Load(xml);
        var rootNode = doc.DocumentElement.ChildNodes[0];
        return (Dictionary<string, object>) parse(rootNode);
    }

    public static Dictionary<string, object> createDictionaryFromBinaryFile(string path)
    {
        using (var reader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
        {
            var buffer = reader.ReadBytes((int) reader.BaseStream.Length);
            return createDictionaryFromBinaryData(buffer);
        }
    }

    public static Dictionary<string, object> createDictionaryFromBinaryData(byte[] binaryData)
    {
        offsetTable.Clear();
        var offsetTableBytes = new List<byte>();
        objectTable.Clear();
        refCount = 0;
        objRefSize = 0;
        offsetByteSize = 0;
        offsetTableOffset = 0;

        var bList = binaryData.ToList();

        var trailer = bList.GetRange(bList.Count - 32, 32);

        parseTrailer(trailer);

        objectTable = bList.GetRange(0, (int) offsetTableOffset);

        offsetTableBytes = bList.GetRange((int) offsetTableOffset, bList.Count - (int) offsetTableOffset - 32);

        parseOffsetTable(offsetTableBytes);

        var magicHeader = BitConverter.ToInt64(objectTable.GetRange(0, 8).ToArray(), 0);

        if (magicHeader != 3472403351741427810)
            throw new Exception("This is not a valid binary plist.");

        return (Dictionary<string, object>) parseBinaryDictionary(0);
    }

    public static byte[] createBinaryDataFromXml(string xml)
    {
        return createBinaryFromDictionary(createDictionaryFromXml(xml));
    }

    public static byte[] createBinaryFromDictionary(Dictionary<string, object> dictionary)
    {
        offsetTable.Clear();
        objectTable.Clear();
        refCount = 0;
        objRefSize = 0;
        offsetByteSize = 0;
        offsetTableOffset = 0;

        var totalRefs = countDictionary(dictionary);

        refCount = totalRefs;

        objRefSize = RegulateNullBytes(BitConverter.GetBytes(refCount)).Length;

        writeBinaryDictionary(dictionary);

        writeBinaryString("bplist00", false);

        offsetTableOffset = objectTable.Count;

        offsetTable.Add(objectTable.Count - 8);

        offsetByteSize = RegulateNullBytes(BitConverter.GetBytes(offsetTable.Last())).Length;

        var offsetBytes = new List<byte>();

        offsetTable.Reverse();

        for (var i = 0; i < offsetTable.Count; i++)
        {
            offsetTable[i] = objectTable.Count - offsetTable[i];
            var buffer = RegulateNullBytes(BitConverter.GetBytes(offsetTable[i]), offsetByteSize);
            Array.Reverse(buffer);
            offsetBytes.AddRange(buffer);
        }

        objectTable.AddRange(offsetBytes);

        objectTable.AddRange(new byte[6]);
        objectTable.Add(Convert.ToByte(offsetByteSize));
        objectTable.Add(Convert.ToByte(objRefSize));
        objectTable.AddRange(BitConverter.GetBytes((long) totalRefs + 1).Reverse());
        objectTable.AddRange(BitConverter.GetBytes((long) 0));
        objectTable.AddRange(BitConverter.GetBytes(offsetTableOffset).Reverse());

        return objectTable.ToArray();
    }

    public static string createXmlFromBinaryData(byte[] binaryData)
    {
        return createXmlFromDictionary(createDictionaryFromBinaryData(binaryData));
    }

    public static string createXmlFromDictionary(Dictionary<string, object> dictionary)
    {
        using (var ms = new MemoryStream())
        {
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = new UTF8Encoding(false);
            xmlWriterSettings.ConformanceLevel = ConformanceLevel.Document;
            xmlWriterSettings.Indent = true;

            var writer = XmlWriter.Create(ms, xmlWriterSettings);
            writer.WriteStartDocument();
            writer.WriteComment("DOCTYPE plist PUBLIC \"-//Apple//DTD PLIST 1.0//EN\" " +
                                "\"http://www.apple.com/DTDs/PropertyList-1.0.dtd\"");
            writer.WriteStartElement("plist");
            writer.WriteAttributeString("version", "1.0");
            writeDictionaryValues(dictionary, writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            writer.Close();
            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }

    #endregion

    #region Private Functions

    private static Dictionary<string, object> parseDictionary(XmlNode node)
    {
        var children = node.ChildNodes;
        if (children.Count % 2 != 0)
            throw new DataMisalignedException("Dictionary elements must have an even number of child nodes");

        var dict = new Dictionary<string, object>();

        for (var i = 0; i < children.Count; i += 2)
        {
            var keynode = children[i];
            var valnode = children[i + 1];

            if (keynode.Name != "key") throw new ApplicationException("expected a key node");

            var result = parse(valnode);

            if (result != null) dict.Add(keynode.InnerText, result);
        }

        return dict;
    }

    private static List<object> parseArray(XmlNode node)
    {
        var array = new List<object>();

        foreach (XmlNode child in node.ChildNodes)
        {
            var result = parse(child);
            if (result != null) array.Add(result);
        }

        return array;
    }

    private static void composeArray(List<object> value, XmlWriter writer)
    {
        writer.WriteStartElement("array");
        foreach (var obj in value) compose(obj, writer);
        writer.WriteEndElement();
    }

    private static object parse(XmlNode node)
    {
        switch (node.Name)
        {
            case "dict":
                return parseDictionary(node);
            case "array":
                return parseArray(node);
            case "string":
                return node.InnerText;
            case "integer":
                return Convert.ToInt32(node.InnerText);
            case "real":
                return Convert.ToDouble(node.InnerText);
            case "false":
                return false;
            case "true":
                return true;
            case "data":
                return Convert.FromBase64String(node.InnerText);
        }

        throw new ApplicationException(string.Format("Plist Node `{0}' is not supported", node.Name));
    }

    private static void compose(object value, XmlWriter writer)
    {
        switch (value.GetType().ToString())
        {
            case "System.Collections.Generic.Dictionary`2[System.String,System.Object]":
                writeDictionaryValues((Dictionary<string, object>) value, writer);
                break;

            case "System.Collections.Generic.List`1[System.Object]":
                composeArray((List<object>) value, writer);
                break;

            case "System.Byte[]":
                writer.WriteElementString("data", Convert.ToBase64String((byte[]) value));
                break;

            case "System.Double":
                writer.WriteElementString("real", value.ToString());
                break;

            case "System.Int32":
                writer.WriteElementString("integer", value.ToString());
                break;

            case "System.String":
                writer.WriteElementString("string", value.ToString());
                break;

            default:
                throw new Exception(string.Format("Value type '{0}' is unhandled", value.GetType().ToString()));
        }
    }

    private static void writeDictionaryValues(Dictionary<string, object> dictionary, XmlWriter writer)
    {
        writer.WriteStartElement("dict");
        foreach (var key in dictionary.Keys)
        {
            var value = dictionary[key];
            writer.WriteElementString("key", key);
            compose(value, writer);
        }

        writer.WriteEndElement();
    }

    private static int countDictionary(Dictionary<string, object> dictionary)
    {
        var count = 0;
        foreach (var key in dictionary.Keys)
        {
            count++;
            switch (dictionary[key].GetType().ToString())
            {
                case "System.Collections.Generic.Dictionary`2[System.String,System.Object]":
                    count += countDictionary((Dictionary<string, object>) dictionary[key]) + 1;
                    break;
                case "System.Collections.Generic.List`1[System.Object]":
                    count += countArray((List<object>) dictionary[key]) + 1;
                    break;
                default:
                    count++;
                    break;
            }
        }

        return count;
    }

    private static int countArray(List<object> array)
    {
        var count = 0;
        foreach (var obj in array)
            switch (obj.GetType().ToString())
            {
                case "System.Collections.Generic.Dictionary`2[System.String,System.Object]":
                    count += countDictionary((Dictionary<string, object>) obj) + 1;
                    break;
                case "System.Collections.Generic.List`1[System.Object]":
                    count += countArray((List<object>) obj) + 1;
                    break;
                default:
                    count++;
                    break;
            }

        return count;
    }

    private static byte[] writeBinaryDictionary(Dictionary<string, object> dictionary)
    {
        var buffer = new List<byte>();
        var header = new List<byte>();
        var refs = new List<int>();
        for (var i = dictionary.Count - 1; i >= 0; i--)
        {
            composeBinary(dictionary.Values.ToArray()[i]);
            offsetTable.Add(objectTable.Count);
            refs.Add(refCount);
            refCount--;
        }

        for (var i = dictionary.Count - 1; i >= 0; i--)
        {
            composeBinary(dictionary.Keys.ToArray()[i]);
            offsetTable.Add(objectTable.Count);
            refs.Add(refCount);
            refCount--;
        }

        if (dictionary.Count < 15)
        {
            header.Add(Convert.ToByte(0xD0 | Convert.ToByte(dictionary.Count)));
        }
        else
        {
            header.Add(0xD0 | 0xf);
            header.AddRange(writeBinaryInteger(dictionary.Count, false));
        }


        foreach (var val in refs)
        {
            var refBuffer = RegulateNullBytes(BitConverter.GetBytes(val), objRefSize);
            Array.Reverse(refBuffer);
            buffer.InsertRange(0, refBuffer);
        }

        buffer.InsertRange(0, header);

        objectTable.InsertRange(0, buffer);

        return buffer.ToArray();
    }

    private static byte[] composeBinaryArray(List<object> objects)
    {
        var buffer = new List<byte>();
        var header = new List<byte>();
        var refs = new List<int>();

        for (var i = objects.Count - 1; i >= 0; i--)
        {
            composeBinary(objects[i]);
            offsetTable.Add(objectTable.Count);
            refs.Add(refCount);
            refCount--;
        }

        if (objects.Count < 15)
        {
            header.Add(Convert.ToByte(0xA0 | Convert.ToByte(objects.Count)));
        }
        else
        {
            header.Add(0xA0 | 0xf);
            header.AddRange(writeBinaryInteger(objects.Count, false));
        }

        foreach (var val in refs)
        {
            var refBuffer = RegulateNullBytes(BitConverter.GetBytes(val), objRefSize);
            Array.Reverse(refBuffer);
            buffer.InsertRange(0, refBuffer);
        }

        buffer.InsertRange(0, header);

        objectTable.InsertRange(0, buffer);

        return buffer.ToArray();
    }

    private static byte[] composeBinary(object obj)
    {
        byte[] value;
        switch (obj.GetType().ToString())
        {
            case "System.Collections.Generic.Dictionary`2[System.String,System.Object]":
                value = writeBinaryDictionary((Dictionary<string, object>) obj);
                return value;

            case "System.Collections.Generic.List`1[System.Object]":
                value = composeBinaryArray((List<object>) obj);
                return value;

            case "System.Byte[]":
                value = writeBinaryByteArray((byte[]) obj);
                return value;

            case "System.Double":
                value = writeBinaryDouble((double) obj);
                return value;

            case "System.Int32":
                value = writeBinaryInteger((int) obj, true);
                return value;

            case "System.String":
                value = writeBinaryString((string) obj, true);
                return value;

            default:
                return new byte[0];
        }
    }

    private static byte[] writeBinaryInteger(int value, bool write)
    {
        var buffer = BitConverter.GetBytes(value).ToList();
        buffer = RegulateNullBytes(buffer.ToArray()).ToList();
        while (buffer.Count != Math.Pow(2, Math.Log(buffer.Count) / Math.Log(2)))
            buffer.Add(0);
        var header = 0x10 | (int) (Math.Log(buffer.Count) / Math.Log(2));

        if (BitConverter.IsLittleEndian)
            buffer.Reverse();

        buffer.Insert(0, Convert.ToByte(header));

        if (write)
            objectTable.InsertRange(0, buffer);

        return buffer.ToArray();
    }

    private static byte[] writeBinaryDouble(double value)
    {
        var buffer = RegulateNullBytes(BitConverter.GetBytes(value), 4).ToList();
        while (buffer.Count != Math.Pow(2, Math.Log(buffer.Count) / Math.Log(2)))
            buffer.Add(0);
        var header = 0x20 | (int) (Math.Log(buffer.Count) / Math.Log(2));

        if (BitConverter.IsLittleEndian)
            buffer.Reverse();

        buffer.Insert(0, Convert.ToByte(header));

        objectTable.InsertRange(0, buffer);

        return buffer.ToArray();
    }

    private static byte[] writeBinaryByteArray(byte[] value)
    {
        var buffer = value.ToList();
        var header = new List<byte>();
        if (value.Length < 15)
        {
            header.Add(Convert.ToByte(0x40 | Convert.ToByte(value.Length)));
        }
        else
        {
            header.Add(0x40 | 0xf);
            header.AddRange(writeBinaryInteger(buffer.Count, false));
        }

        buffer.InsertRange(0, header);

        objectTable.InsertRange(0, buffer);

        return buffer.ToArray();
    }

    private static byte[] writeBinaryString(string value, bool head)
    {
        var buffer = new List<byte>();
        var header = new List<byte>();
        foreach (var chr in value)
            buffer.Add(Convert.ToByte(chr));

        if (head)
        {
            if (value.Length < 15)
            {
                header.Add(Convert.ToByte(0x50 | Convert.ToByte(value.Length)));
            }
            else
            {
                header.Add(0x50 | 0xf);
                header.AddRange(writeBinaryInteger(buffer.Count, false));
            }
        }

        buffer.InsertRange(0, header);

        objectTable.InsertRange(0, buffer);

        return buffer.ToArray();
    }

    private static byte[] RegulateNullBytes(byte[] value)
    {
        return RegulateNullBytes(value, 1);
    }

    private static byte[] RegulateNullBytes(byte[] value, int minBytes)
    {
        Array.Reverse(value);
        var bytes = value.ToList();
        for (var i = 0; i < bytes.Count; i++)
            if (bytes[i] == 0 && bytes.Count > minBytes)
            {
                bytes.Remove(bytes[i]);
                i--;
            }
            else
            {
                break;
            }

        if (bytes.Count < minBytes)
        {
            var dist = minBytes - bytes.Count;
            for (var i = 0; i < dist; i++)
                bytes.Insert(0, 0);
        }

        value = bytes.ToArray();
        Array.Reverse(value);
        return value;
    }

    private static void parseTrailer(List<byte> trailer)
    {
        offsetByteSize = BitConverter.ToInt32(RegulateNullBytes(trailer.GetRange(6, 1).ToArray(), 4), 0);
        objRefSize = BitConverter.ToInt32(RegulateNullBytes(trailer.GetRange(7, 1).ToArray(), 4), 0);
        var refCountBytes = trailer.GetRange(12, 4).ToArray();
        Array.Reverse(refCountBytes);
        refCount = BitConverter.ToInt32(refCountBytes, 0);
        var offsetTableOffsetBytes = trailer.GetRange(24, 8).ToArray();
        Array.Reverse(offsetTableOffsetBytes);
        offsetTableOffset = BitConverter.ToInt64(offsetTableOffsetBytes, 0);
    }

    private static void parseOffsetTable(List<byte> offsetTableBytes)
    {
        for (var i = 0; i < offsetTableBytes.Count; i += offsetByteSize)
        {
            var buffer = offsetTableBytes.GetRange(i, offsetByteSize).ToArray();
            Array.Reverse(buffer);
            offsetTable.Add(BitConverter.ToInt32(RegulateNullBytes(buffer, 4), 0));
        }
    }

    private static object parseBinaryDictionary(int objRef)
    {
        var buffer = new Dictionary<string, object>();
        var refs = new List<int>();
        var refCount = 0;

        var dictByte = objectTable[offsetTable[objRef]];

        refCount = getCount(offsetTable[objRef], dictByte);

        int refStartPosition;

        if (refCount < 15)
            refStartPosition = offsetTable[objRef] + 1;
        else
            refStartPosition = offsetTable[objRef] + 2 + RegulateNullBytes(BitConverter.GetBytes(refCount), 1).Length;

        for (var i = refStartPosition; i < refStartPosition + refCount * 2 * objRefSize; i += objRefSize)
        {
            var refBuffer = objectTable.GetRange(i, objRefSize).ToArray();
            Array.Reverse(refBuffer);
            refs.Add(BitConverter.ToInt32(RegulateNullBytes(refBuffer, 4), 0));
        }

        for (var i = 0; i < refCount; i++) buffer.Add((string) parseBinary(refs[i]), parseBinary(refs[i + refCount]));

        return buffer;
    }

    private static object parseBinaryArray(int objRef)
    {
        var buffer = new List<object>();
        var refs = new List<int>();
        var refCount = 0;

        var arrayByte = objectTable[offsetTable[objRef]];

        refCount = getCount(offsetTable[objRef], arrayByte);

        int refStartPosition;

        if (refCount < 15)
            refStartPosition = offsetTable[objRef] + 1;
        else
            refStartPosition = offsetTable[objRef] + 2 + RegulateNullBytes(BitConverter.GetBytes(refCount), 1).Length;

        for (var i = refStartPosition; i < refStartPosition + refCount * 2 * objRefSize; i += objRefSize)
        {
            var refBuffer = objectTable.GetRange(i, objRefSize).ToArray();
            Array.Reverse(refBuffer);
            refs.Add(BitConverter.ToInt32(RegulateNullBytes(refBuffer, 4), 0));
        }

        for (var i = 0; i < refCount; i++) buffer.Add(parseBinary(refs[i]));

        return buffer;
    }

    private static int getCount(int bytePosition, byte headerByte)
    {
        var headerByteTrail = Convert.ToByte(headerByte & 0xf);
        if (headerByteTrail < 15)
            return headerByteTrail;
        return (int) parseBinaryInt(bytePosition + 1);
    }

    private static object parseBinary(int objRef)
    {
        var header = objectTable[offsetTable[objRef]];
        switch (header & 0xF0)
        {
            case 0x10:
            {
                return parseBinaryInt(offsetTable[objRef]);
            }
            case 0x20:
            {
                return parseBinaryReal(offsetTable[objRef]);
            }
            case 0x40:
            {
                return parseBinaryByteArray(offsetTable[objRef]);
            }
            case 0x50:
            {
                return parseBinaryString(offsetTable[objRef]);
            }
            case 0xD0:
            {
                return parseBinaryDictionary(objRef);
            }
            case 0xA0:
            {
                return parseBinaryArray(objRef);
            }
        }

        throw new Exception("This type is not supported");
    }

    private static object parseBinaryInt(int headerPosition)
    {
        var header = objectTable[headerPosition];
        var byteCount = (int) Math.Pow(2, header & 0xf);
        var buffer = objectTable.GetRange(headerPosition + 1, byteCount).ToArray();
        Array.Reverse(buffer);

        return BitConverter.ToInt32(RegulateNullBytes(buffer, 4), 0);
    }

    private static object parseBinaryReal(int headerPosition)
    {
        var header = objectTable[headerPosition];
        var byteCount = (int) Math.Pow(2, header & 0xf);
        var buffer = objectTable.GetRange(headerPosition + 1, byteCount).ToArray();
        Array.Reverse(buffer);

        return BitConverter.ToDouble(RegulateNullBytes(buffer, 8), 0);
    }

    private static object parseBinaryString(int headerPosition)
    {
        var headerByte = objectTable[headerPosition];
        var charCount = getCount(headerPosition, headerByte);
        int charStartPosition;
        if (charCount < 15)
            charStartPosition = headerPosition + 1;
        else
            charStartPosition = headerPosition + 2 + RegulateNullBytes(BitConverter.GetBytes(charCount), 1).Length;
        var buffer = "";
        foreach (var byt in objectTable.GetRange(charStartPosition, charCount)) buffer += Convert.ToChar(byt);
        return buffer;
    }

    private static object parseBinaryByteArray(int headerPosition)
    {
        var headerByte = objectTable[headerPosition];
        var byteCount = getCount(headerPosition, headerByte);
        int byteStartPosition;
        if (byteCount < 15)
            byteStartPosition = headerPosition + 1;
        else
            byteStartPosition = headerPosition + 2 + RegulateNullBytes(BitConverter.GetBytes(byteCount), 1).Length;
        return objectTable.GetRange(byteStartPosition, byteCount).ToArray();
    }

    #endregion
}