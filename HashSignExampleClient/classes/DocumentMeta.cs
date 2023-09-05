namespace HashSignExampleClient.classes
{
    public class DocumentMeta
    {
        public string path { get; set; }
        public string filename { get; set; }
        public int startZeroBytes { get; set; }
        public int endZeroBytes { get; set; }
        public byte[] signedAttributesBytes { get; set; }
        public byte[] preparedPdf { get; set; }
        public byte[] preparedPdfNo0s { get; set; }
        public string hash { get; set; }

    }
}
