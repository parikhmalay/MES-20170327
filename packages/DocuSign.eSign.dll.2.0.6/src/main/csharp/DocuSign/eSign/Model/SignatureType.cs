using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace DocuSign.eSign.Model
{

    /// <summary>
    /// 
    /// </summary>
    [DataContract]
    public partial class SignatureType :  IEquatable<SignatureType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SignatureType" /> class.
        /// </summary>
        public SignatureType()
        {
            
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DataMember(Name="type", EmitDefaultValue=false)]
        public string Type { get; set; }
  
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DataMember(Name="isDefault", EmitDefaultValue=false)]
        public string IsDefault { get; set; }
  
        
  
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SignatureType {\n");
            sb.Append("  Type: ").Append(Type).Append("\n");
            sb.Append("  IsDefault: ").Append(IsDefault).Append("\n");
            
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="obj">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object obj)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            return this.Equals(obj as SignatureType);
        }

        /// <summary>
        /// Returns true if SignatureType instances are equal
        /// </summary>
        /// <param name="other">Instance of SignatureType to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SignatureType other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.Type == other.Type ||
                    this.Type != null &&
                    this.Type.Equals(other.Type)
                ) && 
                (
                    this.IsDefault == other.IsDefault ||
                    this.IsDefault != null &&
                    this.IsDefault.Equals(other.IsDefault)
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            // credit: http://stackoverflow.com/a/263416/677735
            unchecked // Overflow is fine, just wrap
            {
                int hash = 41;
                // Suitable nullity checks etc, of course :)
                
                if (this.Type != null)
                    hash = hash * 59 + this.Type.GetHashCode();
                
                if (this.IsDefault != null)
                    hash = hash * 59 + this.IsDefault.GetHashCode();
                
                return hash;
            }
        }

    }
}
