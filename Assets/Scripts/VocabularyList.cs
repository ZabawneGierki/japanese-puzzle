using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[Serializable]
[XmlRoot("words")]
public class VocabularyList
{
    [XmlElement("word")]
    public List<WordEntry> Words = new List<WordEntry>();
}