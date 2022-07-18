using System.IO;

namespace Data.Models.CustomModels
{
    public class NPOIMemoryStream : MemoryStream
    {
        public NPOIMemoryStream()
        {
            AllowClose = true;
        }

        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}
