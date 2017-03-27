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
    public partial class RecipientEmailNotification :  IEquatable<RecipientEmailNotification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RecipientEmailNotification" /> class.
        /// </summary>
        public RecipientEmailNotification()
        {
            
        }

        
        /// <summary>
        /// Specifies the subject of the email that is sent to all recipients.\n\nSee [ML:Template Email Subject Merge Fields] for information about adding merge field information to the email subject.
        /// </summary>
        /// <value>Specifies the subject of the email that is sent to all recipients.\n\nSee [ML:Template Email Subject Merge Fields] for information about adding merge field information to the email subject.</value>
        [DataMember(Name="emailSubject", EmitDefaultValue=false)]
        public string EmailSubject { get; set; }
  
        
        /// <summary>
        /// Specifies the email body of the message sent to the recipient. \n\nMaximum length: 10000 characters.
        /// </summary>
        /// <value>Specifies the email body of the message sent to the recipient. \n\nMaximum length: 10000 characters.</value>
        [DataMember(Name="emailBody", EmitDefaultValue=false)]
        public string EmailBody { get; set; }
  
        
        /// <summary>
        /// A simple type enumeration of the language used. The supported languages, with the language value shown in parenthesis, are: Arabic (ar), Bahasa Indonesia (id), Bahasa Melayu (ms) Bulgarian (bg), Czech (cs), Chinese Simplified (zh_CN), Chinese Traditional (zh_TW), Croatian (hr), Danish (da), Dutch (nl), English US (en), English UK (en_GB), Estonian (et), Farsi (fa), Finnish (fi), French (fr), French Canada (fr_CA), German (de), Greek (el), Hebrew (he), Hindi (hi), Hungarian (hu), Italian (it), Japanese (ja), Korean (ko), Latvian (lv), Lithuanian (lt), Norwegian (no), Polish (pl), Portuguese (pt), Portuguese Brazil (pt_BR), Romanian (ro),Russian (ru), Serbian (sr), Slovak (sk), Slovenian (sl), Spanish (es),Spanish Latin America (es_MX), Swedish (sv), Thai (th), Turkish (tr), Ukrainian (uk), and Vietnamese (vi).
        /// </summary>
        /// <value>A simple type enumeration of the language used. The supported languages, with the language value shown in parenthesis, are: Arabic (ar), Bahasa Indonesia (id), Bahasa Melayu (ms) Bulgarian (bg), Czech (cs), Chinese Simplified (zh_CN), Chinese Traditional (zh_TW), Croatian (hr), Danish (da), Dutch (nl), English US (en), English UK (en_GB), Estonian (et), Farsi (fa), Finnish (fi), French (fr), French Canada (fr_CA), German (de), Greek (el), Hebrew (he), Hindi (hi), Hungarian (hu), Italian (it), Japanese (ja), Korean (ko), Latvian (lv), Lithuanian (lt), Norwegian (no), Polish (pl), Portuguese (pt), Portuguese Brazil (pt_BR), Romanian (ro),Russian (ru), Serbian (sr), Slovak (sk), Slovenian (sl), Spanish (es),Spanish Latin America (es_MX), Swedish (sv), Thai (th), Turkish (tr), Ukrainian (uk), and Vietnamese (vi).</value>
        [DataMember(Name="supportedLanguage", EmitDefaultValue=false)]
        public string SupportedLanguage { get; set; }
  
        
  
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class RecipientEmailNotification {\n");
            sb.Append("  EmailSubject: ").Append(EmailSubject).Append("\n");
            sb.Append("  EmailBody: ").Append(EmailBody).Append("\n");
            sb.Append("  SupportedLanguage: ").Append(SupportedLanguage).Append("\n");
            
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
            return this.Equals(obj as RecipientEmailNotification);
        }

        /// <summary>
        /// Returns true if RecipientEmailNotification instances are equal
        /// </summary>
        /// <param name="other">Instance of RecipientEmailNotification to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(RecipientEmailNotification other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.EmailSubject == other.EmailSubject ||
                    this.EmailSubject != null &&
                    this.EmailSubject.Equals(other.EmailSubject)
                ) && 
                (
                    this.EmailBody == other.EmailBody ||
                    this.EmailBody != null &&
                    this.EmailBody.Equals(other.EmailBody)
                ) && 
                (
                    this.SupportedLanguage == other.SupportedLanguage ||
                    this.SupportedLanguage != null &&
                    this.SupportedLanguage.Equals(other.SupportedLanguage)
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
                
                if (this.EmailSubject != null)
                    hash = hash * 59 + this.EmailSubject.GetHashCode();
                
                if (this.EmailBody != null)
                    hash = hash * 59 + this.EmailBody.GetHashCode();
                
                if (this.SupportedLanguage != null)
                    hash = hash * 59 + this.SupportedLanguage.GetHashCode();
                
                return hash;
            }
        }

    }
}
