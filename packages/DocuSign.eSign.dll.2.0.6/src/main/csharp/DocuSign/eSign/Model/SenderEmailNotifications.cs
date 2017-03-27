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
    public partial class SenderEmailNotifications :  IEquatable<SenderEmailNotifications>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SenderEmailNotifications" /> class.
        /// </summary>
        public SenderEmailNotifications()
        {
            
        }

        
        /// <summary>
        /// When set to **true**, the user receives notification that the envelope has been completed.
        /// </summary>
        /// <value>When set to **true**, the user receives notification that the envelope has been completed.</value>
        [DataMember(Name="envelopeComplete", EmitDefaultValue=false)]
        public string EnvelopeComplete { get; set; }
  
        
        /// <summary>
        /// When set to **true**, the sender receives notification if the signer changes.
        /// </summary>
        /// <value>When set to **true**, the sender receives notification if the signer changes.</value>
        [DataMember(Name="changedSigner", EmitDefaultValue=false)]
        public string ChangedSigner { get; set; }
  
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DataMember(Name="senderEnvelopeDeclined", EmitDefaultValue=false)]
        public string SenderEnvelopeDeclined { get; set; }
  
        
        /// <summary>
        /// When set to **true**, the user receives notification if consent is withdrawn.
        /// </summary>
        /// <value>When set to **true**, the user receives notification if consent is withdrawn.</value>
        [DataMember(Name="withdrawnConsent", EmitDefaultValue=false)]
        public string WithdrawnConsent { get; set; }
  
        
        /// <summary>
        /// When set to **true**, the sender receives notification that the recipient viewed the enveloper.
        /// </summary>
        /// <value>When set to **true**, the sender receives notification that the recipient viewed the enveloper.</value>
        [DataMember(Name="recipientViewed", EmitDefaultValue=false)]
        public string RecipientViewed { get; set; }
  
        
        /// <summary>
        /// When set to **true**, the sender receives notification if the delivery of the envelope fails.
        /// </summary>
        /// <value>When set to **true**, the sender receives notification if the delivery of the envelope fails.</value>
        [DataMember(Name="deliveryFailed", EmitDefaultValue=false)]
        public string DeliveryFailed { get; set; }
  
        
        /// <summary>
        /// When set to **true**, the user receives notification if the offline signing failed.
        /// </summary>
        /// <value>When set to **true**, the user receives notification if the offline signing failed.</value>
        [DataMember(Name="offlineSigningFailed", EmitDefaultValue=false)]
        public string OfflineSigningFailed { get; set; }
  
        
  
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class SenderEmailNotifications {\n");
            sb.Append("  EnvelopeComplete: ").Append(EnvelopeComplete).Append("\n");
            sb.Append("  ChangedSigner: ").Append(ChangedSigner).Append("\n");
            sb.Append("  SenderEnvelopeDeclined: ").Append(SenderEnvelopeDeclined).Append("\n");
            sb.Append("  WithdrawnConsent: ").Append(WithdrawnConsent).Append("\n");
            sb.Append("  RecipientViewed: ").Append(RecipientViewed).Append("\n");
            sb.Append("  DeliveryFailed: ").Append(DeliveryFailed).Append("\n");
            sb.Append("  OfflineSigningFailed: ").Append(OfflineSigningFailed).Append("\n");
            
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
            return this.Equals(obj as SenderEmailNotifications);
        }

        /// <summary>
        /// Returns true if SenderEmailNotifications instances are equal
        /// </summary>
        /// <param name="other">Instance of SenderEmailNotifications to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(SenderEmailNotifications other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.EnvelopeComplete == other.EnvelopeComplete ||
                    this.EnvelopeComplete != null &&
                    this.EnvelopeComplete.Equals(other.EnvelopeComplete)
                ) && 
                (
                    this.ChangedSigner == other.ChangedSigner ||
                    this.ChangedSigner != null &&
                    this.ChangedSigner.Equals(other.ChangedSigner)
                ) && 
                (
                    this.SenderEnvelopeDeclined == other.SenderEnvelopeDeclined ||
                    this.SenderEnvelopeDeclined != null &&
                    this.SenderEnvelopeDeclined.Equals(other.SenderEnvelopeDeclined)
                ) && 
                (
                    this.WithdrawnConsent == other.WithdrawnConsent ||
                    this.WithdrawnConsent != null &&
                    this.WithdrawnConsent.Equals(other.WithdrawnConsent)
                ) && 
                (
                    this.RecipientViewed == other.RecipientViewed ||
                    this.RecipientViewed != null &&
                    this.RecipientViewed.Equals(other.RecipientViewed)
                ) && 
                (
                    this.DeliveryFailed == other.DeliveryFailed ||
                    this.DeliveryFailed != null &&
                    this.DeliveryFailed.Equals(other.DeliveryFailed)
                ) && 
                (
                    this.OfflineSigningFailed == other.OfflineSigningFailed ||
                    this.OfflineSigningFailed != null &&
                    this.OfflineSigningFailed.Equals(other.OfflineSigningFailed)
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
                
                if (this.EnvelopeComplete != null)
                    hash = hash * 59 + this.EnvelopeComplete.GetHashCode();
                
                if (this.ChangedSigner != null)
                    hash = hash * 59 + this.ChangedSigner.GetHashCode();
                
                if (this.SenderEnvelopeDeclined != null)
                    hash = hash * 59 + this.SenderEnvelopeDeclined.GetHashCode();
                
                if (this.WithdrawnConsent != null)
                    hash = hash * 59 + this.WithdrawnConsent.GetHashCode();
                
                if (this.RecipientViewed != null)
                    hash = hash * 59 + this.RecipientViewed.GetHashCode();
                
                if (this.DeliveryFailed != null)
                    hash = hash * 59 + this.DeliveryFailed.GetHashCode();
                
                if (this.OfflineSigningFailed != null)
                    hash = hash * 59 + this.OfflineSigningFailed.GetHashCode();
                
                return hash;
            }
        }

    }
}
