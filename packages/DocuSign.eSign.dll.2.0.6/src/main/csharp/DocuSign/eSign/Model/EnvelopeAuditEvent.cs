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
    public partial class EnvelopeAuditEvent :  IEquatable<EnvelopeAuditEvent>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvelopeAuditEvent" /> class.
        /// </summary>
        public EnvelopeAuditEvent()
        {
            
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DataMember(Name="eventFields", EmitDefaultValue=false)]
        public List<NameValue> EventFields { get; set; }
  
        
  
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class EnvelopeAuditEvent {\n");
            sb.Append("  EventFields: ").Append(EventFields).Append("\n");
            
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
            return this.Equals(obj as EnvelopeAuditEvent);
        }

        /// <summary>
        /// Returns true if EnvelopeAuditEvent instances are equal
        /// </summary>
        /// <param name="other">Instance of EnvelopeAuditEvent to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(EnvelopeAuditEvent other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.EventFields == other.EventFields ||
                    this.EventFields != null &&
                    this.EventFields.SequenceEqual(other.EventFields)
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
                
                if (this.EventFields != null)
                    hash = hash * 59 + this.EventFields.GetHashCode();
                
                return hash;
            }
        }

    }
}
