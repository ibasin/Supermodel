using System;
using System.Data.Entity.ModelConfiguration;
using System.IO;
using System.Linq;

using Newtonsoft.Json;

namespace Supermodel.DDD.Models.Domain
{
    public class BinaryFile : IComparable
    {
        public string Name { get; set; }
        [JsonIgnore] public string Extension { get { return Path.GetExtension(Name); } }
        [JsonIgnore] public string FileNameWithoutExtension { get { return Path.GetFileNameWithoutExtension(Name); } }
        public byte[] BinaryContent { get; set; }

        [JsonIgnore] public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(Name) && (BinaryContent == null || BinaryContent.Length == 0); }
        }

        public int CompareTo(object obj)
        {
            var typedObj = (BinaryFile)obj;
            if (Name == null) return 0; //if we are an empty object, we say it equals b/c then we do not override db value
            var result = string.CompareOrdinal(Name, typedObj.Name);
            if (result != 0) return result;
            return BinaryContent.GetHashCode().CompareTo(typedObj.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as BinaryFile);
        }

        public bool Equals(BinaryFile other)
        {
            return other != null &&
                   (this.Name == null && other.Name == null ||
                    this.Name != null && this.Name.Equals(other.Name)) &&
                   (this.BinaryContent == null && other.BinaryContent == null ||
                    this.BinaryContent != null && this.BinaryContent.SequenceEqual(other.BinaryContent));
        }

        public override int GetHashCode()
        {
            // http://stackoverflow.com/a/263416/39396
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                if (this.Name != null)
                    hash = hash * 23 + this.Name.GetHashCode();
                if (this.BinaryContent != null)
                    hash = hash * 23 + this.BinaryContent.GetHashCode();
                return hash;
            }
        }

        public void Empty()
        {
            this.Name = null;
            this.BinaryContent = null;
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class BinaryFileTypeMapping : ComplexTypeConfiguration<BinaryFile>
    {
        public BinaryFileTypeMapping()
        {
            Property(o => o.Name);
            Property(o => o.BinaryContent);
        }
    }
}
