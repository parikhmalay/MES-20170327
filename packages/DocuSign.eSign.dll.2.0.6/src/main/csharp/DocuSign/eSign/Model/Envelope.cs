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
    public partial class Envelope :  IEquatable<Envelope>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Envelope" /> class.
        /// </summary>
        public Envelope()
        {
            
        }

        
        /// <summary>
        /// Used to identify an envelope. The id is a sender-generated value and is valid in the DocuSign system for 7 days. It is recommended that a transaction ID is used for offline signing to ensure that an envelope is not sent multiple times. The `transactionId` property can be used determine an envelope's status (i.e. was it created or not) in cases where the internet connection was lost before the envelope status was returned.
        /// </summary>
        /// <value>Used to identify an envelope. The id is a sender-generated value and is valid in the DocuSign system for 7 days. It is recommended that a transaction ID is used for offline signing to ensure that an envelope is not sent multiple times. The `transactionId` property can be used determine an envelope's status (i.e. was it created or not) in cases where the internet connection was lost before the envelope status was returned.</value>
        [DataMember(Name="transactionId", EmitDefaultValue=false)]
        public string TransactionId { get; set; }
  
        
        /// <summary>
        /// Indicates the envelope status. Valid values are:\n\n* sent - The envelope is sent to the recipients. \n*created - The envelope is saved as a draft and can be modified and sent later.
        /// </summary>
        /// <value>Indicates the envelope status. Valid values are:\n\n* sent - The envelope is sent to the recipients. \n*created - The envelope is saved as a draft and can be modified and sent later.</value>
        [DataMember(Name="status", EmitDefaultValue=false)]
        public string Status { get; set; }
  
        
        /// <summary>
        /// Contains a URI for an endpoint that you can use to retrieve the documents.
        /// </summary>
        /// <value>Contains a URI for an endpoint that you can use to retrieve the documents.</value>
        [DataMember(Name="documentsUri", EmitDefaultValue=false)]
        public string DocumentsUri { get; set; }
  
        
        /// <summary>
        /// Contains a URI for an endpoint that you can use to retrieve the recipients.
        /// </summary>
        /// <value>Contains a URI for an endpoint that you can use to retrieve the recipients.</value>
        [DataMember(Name="recipientsUri", EmitDefaultValue=false)]
        public string RecipientsUri { get; set; }
  
        
        /// <summary>
        /// When set to **true**, the envelope is queued for processing and the value of the `status` property is set to 'Processing'. Additionally, get status calls return 'Processing' until completed.
        /// </summary>
        /// <value>When set to **true**, the envelope is queued for processing and the value of the `status` property is set to 'Processing'. Additionally, get status calls return 'Processing' until completed.</value>
        [DataMember(Name="asynchronous", EmitDefaultValue=false)]
        public string Asynchronous { get; set; }
  
        
        /// <summary>
        /// Contains a URI for an endpoint that you can use to retrieve the envelope or envelopes.
        /// </summary>
        /// <value>Contains a URI for an endpoint that you can use to retrieve the envelope or envelopes.</value>
        [DataMember(Name="envelopeUri", EmitDefaultValue=false)]
        public string EnvelopeUri { get; set; }
  
        
        /// <summary>
        /// Specifies the subject of the email that is sent to all recipients.\n\nSee [ML:Template Email Subject Merge Fields] for information about adding merge field information to the email subject.
        /// </summary>
        /// <value>Specifies the subject of the email that is sent to all recipients.\n\nSee [ML:Template Email Subject Merge Fields] for information about adding merge field information to the email subject.</value>
        [DataMember(Name="emailSubject", EmitDefaultValue=false)]
        public string EmailSubject { get; set; }
  
        
        /// <summary>
        /// This is the same as the email body. If specified it is included in email body for all envelope recipients.
        /// </summary>
        /// <value>This is the same as the email body. If specified it is included in email body for all envelope recipients.</value>
        [DataMember(Name="emailBlurb", EmitDefaultValue=false)]
        public string EmailBlurb { get; set; }
  
        
        /// <summary>
        /// The envelope ID of the envelope status that failed to post.
        /// </summary>
        /// <value>The envelope ID of the envelope status that failed to post.</value>
        [DataMember(Name="envelopeId", EmitDefaultValue=false)]
        public string EnvelopeId { get; set; }
  
        
        /// <summary>
        /// Specifies the physical location where the signing takes place. It can have two enumeration values; InPerson and Online. The default value is Online.
        /// </summary>
        /// <value>Specifies the physical location where the signing takes place. It can have two enumeration values; InPerson and Online. The default value is Online.</value>
        [DataMember(Name="signingLocation", EmitDefaultValue=false)]
        public string SigningLocation { get; set; }
  
        
        /// <summary>
        /// Contains a URI for an endpoint that you can use to retrieve the custom fields.
        /// </summary>
        /// <value>Contains a URI for an endpoint that you can use to retrieve the custom fields.</value>
        [DataMember(Name="customFieldsUri", EmitDefaultValue=false)]
        public string CustomFieldsUri { get; set; }
  
        
        /// <summary>
        /// When set to **true**, Envelope ID Stamping is enabled.
        /// </summary>
        /// <value>When set to **true**, Envelope ID Stamping is enabled.</value>
        [DataMember(Name="envelopeIdStamping", EmitDefaultValue=false)]
        public string EnvelopeIdStamping { get; set; }
  
        
        /// <summary>
        /// Specifies the Authoritative copy feature. If set to true the Authoritative copy feature is enabled.
        /// </summary>
        /// <value>Specifies the Authoritative copy feature. If set to true the Authoritative copy feature is enabled.</value>
        [DataMember(Name="authoritativeCopy", EmitDefaultValue=false)]
        public string AuthoritativeCopy { get; set; }
  
        
        /// <summary>
        /// Gets or Sets Notification
        /// </summary>
        [DataMember(Name="notification", EmitDefaultValue=false)]
        public Notification Notification { get; set; }
  
        
        /// <summary>
        /// Contains a URI for an endpoint that you can use to retrieve the notifications.
        /// </summary>
        /// <value>Contains a URI for an endpoint that you can use to retrieve the notifications.</value>
        [DataMember(Name="notificationUri", EmitDefaultValue=false)]
        public string NotificationUri { get; set; }
  
        
        /// <summary>
        /// When set to **true**, documents with tabs can only be viewed by signers that have a tab on that document. Recipients that have an administrative role (Agent, Editor, or Intermediaries) or informational role (Certified Deliveries or Carbon Copies) can always see all the documents in an envelope, unless they are specifically excluded using this setting when an envelope is sent. Documents that do not have tabs are always visible to all recipients, unless they are specifically excluded using this setting when an envelope is sent.\n\nYour account must have Document Visibility enabled to use this.
        /// </summary>
        /// <value>When set to **true**, documents with tabs can only be viewed by signers that have a tab on that document. Recipients that have an administrative role (Agent, Editor, or Intermediaries) or informational role (Certified Deliveries or Carbon Copies) can always see all the documents in an envelope, unless they are specifically excluded using this setting when an envelope is sent. Documents that do not have tabs are always visible to all recipients, unless they are specifically excluded using this setting when an envelope is sent.\n\nYour account must have Document Visibility enabled to use this.</value>
        [DataMember(Name="enforceSignerVisibility", EmitDefaultValue=false)]
        public string EnforceSignerVisibility { get; set; }
  
        
        /// <summary>
        /// When set to **true**, the signer is allowed to print the document and sign it on paper.
        /// </summary>
        /// <value>When set to **true**, the signer is allowed to print the document and sign it on paper.</value>
        [DataMember(Name="enableWetSign", EmitDefaultValue=false)]
        public string EnableWetSign { get; set; }
  
        
        /// <summary>
        /// When set to **true**, Document Markup is enabled for envelope. Account must have Document Markup enabled to use this
        /// </summary>
        /// <value>When set to **true**, Document Markup is enabled for envelope. Account must have Document Markup enabled to use this</value>
        [DataMember(Name="allowMarkup", EmitDefaultValue=false)]
        public string AllowMarkup { get; set; }
  
        
        /// <summary>
        /// When set to **true**, the recipient can redirect an envelope to a more appropriate recipient.
        /// </summary>
        /// <value>When set to **true**, the recipient can redirect an envelope to a more appropriate recipient.</value>
        [DataMember(Name="allowReassign", EmitDefaultValue=false)]
        public string AllowReassign { get; set; }
  
        
        /// <summary>
        /// Indicates the date and time the item was created.
        /// </summary>
        /// <value>Indicates the date and time the item was created.</value>
        [DataMember(Name="createdDateTime", EmitDefaultValue=false)]
        public string CreatedDateTime { get; set; }
  
        
        /// <summary>
        /// The date and time the item was last modified.
        /// </summary>
        /// <value>The date and time the item was last modified.</value>
        [DataMember(Name="lastModifiedDateTime", EmitDefaultValue=false)]
        public string LastModifiedDateTime { get; set; }
  
        
        /// <summary>
        /// Reserved: For DocuSign use only.
        /// </summary>
        /// <value>Reserved: For DocuSign use only.</value>
        [DataMember(Name="deliveredDateTime", EmitDefaultValue=false)]
        public string DeliveredDateTime { get; set; }
  
        
        /// <summary>
        /// The date and time the envelope was sent.
        /// </summary>
        /// <value>The date and time the envelope was sent.</value>
        [DataMember(Name="sentDateTime", EmitDefaultValue=false)]
        public string SentDateTime { get; set; }
  
        
        /// <summary>
        /// Specifies the date and time this item was completed.
        /// </summary>
        /// <value>Specifies the date and time this item was completed.</value>
        [DataMember(Name="completedDateTime", EmitDefaultValue=false)]
        public string CompletedDateTime { get; set; }
  
        
        /// <summary>
        /// The date and time the envelope or template was voided.
        /// </summary>
        /// <value>The date and time the envelope or template was voided.</value>
        [DataMember(Name="voidedDateTime", EmitDefaultValue=false)]
        public string VoidedDateTime { get; set; }
  
        
        /// <summary>
        /// The reason the envelope or template was voided.
        /// </summary>
        /// <value>The reason the envelope or template was voided.</value>
        [DataMember(Name="voidedReason", EmitDefaultValue=false)]
        public string VoidedReason { get; set; }
  
        
        /// <summary>
        /// Specifies the data and time the item was deleted.
        /// </summary>
        /// <value>Specifies the data and time the item was deleted.</value>
        [DataMember(Name="deletedDateTime", EmitDefaultValue=false)]
        public string DeletedDateTime { get; set; }
  
        
        /// <summary>
        /// The date and time the recipient declined the document.
        /// </summary>
        /// <value>The date and time the recipient declined the document.</value>
        [DataMember(Name="declinedDateTime", EmitDefaultValue=false)]
        public string DeclinedDateTime { get; set; }
  
        
        /// <summary>
        /// The data and time the status changed.
        /// </summary>
        /// <value>The data and time the status changed.</value>
        [DataMember(Name="statusChangedDateTime", EmitDefaultValue=false)]
        public string StatusChangedDateTime { get; set; }
  
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DataMember(Name="documentsCombinedUri", EmitDefaultValue=false)]
        public string DocumentsCombinedUri { get; set; }
  
        
        /// <summary>
        /// Retrieves a URI for an endpoint that allows you to easily retrieve certificate information.
        /// </summary>
        /// <value>Retrieves a URI for an endpoint that allows you to easily retrieve certificate information.</value>
        [DataMember(Name="certificateUri", EmitDefaultValue=false)]
        public string CertificateUri { get; set; }
  
        
        /// <summary>
        /// Contains a URI for an endpoint which you can use to retrieve the templates.
        /// </summary>
        /// <value>Contains a URI for an endpoint which you can use to retrieve the templates.</value>
        [DataMember(Name="templatesUri", EmitDefaultValue=false)]
        public string TemplatesUri { get; set; }
  
        
        /// <summary>
        /// When set to **true**, prevents senders from changing the contents of `emailBlurb` and `emailSubject` properties for the envelope. \n\nAdditionally, this prevents users from making changes to the contents of `emailBlurb` and `emailSubject` properties when correcting envelopes. \n\nHowever, if the `messageLock` node is set to true**** and the `emailSubject` property is empty, senders and correctors are able to add a subject to the envelope.
        /// </summary>
        /// <value>When set to **true**, prevents senders from changing the contents of `emailBlurb` and `emailSubject` properties for the envelope. \n\nAdditionally, this prevents users from making changes to the contents of `emailBlurb` and `emailSubject` properties when correcting envelopes. \n\nHowever, if the `messageLock` node is set to true**** and the `emailSubject` property is empty, senders and correctors are able to add a subject to the envelope.</value>
        [DataMember(Name="messageLock", EmitDefaultValue=false)]
        public string MessageLock { get; set; }
  
        
        /// <summary>
        /// When set to **true**, prevents senders from changing, correcting, or deleting the recipient information for the envelope.
        /// </summary>
        /// <value>When set to **true**, prevents senders from changing, correcting, or deleting the recipient information for the envelope.</value>
        [DataMember(Name="recipientsLock", EmitDefaultValue=false)]
        public string RecipientsLock { get; set; }
  
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DataMember(Name="brandLock", EmitDefaultValue=false)]
        public string BrandLock { get; set; }
  
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DataMember(Name="brandId", EmitDefaultValue=false)]
        public string BrandId { get; set; }
  
        
        /// <summary>
        /// When set to **true**, the disclosure is shown to recipients in accordance with the account’s Electronic Record and Signature Disclosure frequency setting. When set to **false**, the Electronic Record and Signature Disclosure is not shown to any envelope recipients. \n\nIf the `useDisclosure` property is not set, then the account's normal disclosure setting is used and the value of the `useDisclosure` property is not returned in responses when getting envelope information.
        /// </summary>
        /// <value>When set to **true**, the disclosure is shown to recipients in accordance with the account’s Electronic Record and Signature Disclosure frequency setting. When set to **false**, the Electronic Record and Signature Disclosure is not shown to any envelope recipients. \n\nIf the `useDisclosure` property is not set, then the account's normal disclosure setting is used and the value of the `useDisclosure` property is not returned in responses when getting envelope information.</value>
        [DataMember(Name="useDisclosure", EmitDefaultValue=false)]
        public string UseDisclosure { get; set; }
  
        
        /// <summary>
        /// Gets or Sets EmailSettings
        /// </summary>
        [DataMember(Name="emailSettings", EmitDefaultValue=false)]
        public EmailSettings EmailSettings { get; set; }
  
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DataMember(Name="purgeState", EmitDefaultValue=false)]
        public string PurgeState { get; set; }
  
        
        /// <summary>
        /// Gets or Sets LockInformation
        /// </summary>
        [DataMember(Name="lockInformation", EmitDefaultValue=false)]
        public LockInformation LockInformation { get; set; }
  
        
        /// <summary>
        /// When set to **true**, indicates that this module is enabled on the account.
        /// </summary>
        /// <value>When set to **true**, indicates that this module is enabled on the account.</value>
        [DataMember(Name="is21CFRPart11", EmitDefaultValue=false)]
        public string Is21CFRPart11 { get; set; }
  
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [DataMember(Name="isUniversalSignatureEnvelope", EmitDefaultValue=false)]
        public string IsUniversalSignatureEnvelope { get; set; }
  
        
  
        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Envelope {\n");
            sb.Append("  TransactionId: ").Append(TransactionId).Append("\n");
            sb.Append("  Status: ").Append(Status).Append("\n");
            sb.Append("  DocumentsUri: ").Append(DocumentsUri).Append("\n");
            sb.Append("  RecipientsUri: ").Append(RecipientsUri).Append("\n");
            sb.Append("  Asynchronous: ").Append(Asynchronous).Append("\n");
            sb.Append("  EnvelopeUri: ").Append(EnvelopeUri).Append("\n");
            sb.Append("  EmailSubject: ").Append(EmailSubject).Append("\n");
            sb.Append("  EmailBlurb: ").Append(EmailBlurb).Append("\n");
            sb.Append("  EnvelopeId: ").Append(EnvelopeId).Append("\n");
            sb.Append("  SigningLocation: ").Append(SigningLocation).Append("\n");
            sb.Append("  CustomFieldsUri: ").Append(CustomFieldsUri).Append("\n");
            sb.Append("  EnvelopeIdStamping: ").Append(EnvelopeIdStamping).Append("\n");
            sb.Append("  AuthoritativeCopy: ").Append(AuthoritativeCopy).Append("\n");
            sb.Append("  Notification: ").Append(Notification).Append("\n");
            sb.Append("  NotificationUri: ").Append(NotificationUri).Append("\n");
            sb.Append("  EnforceSignerVisibility: ").Append(EnforceSignerVisibility).Append("\n");
            sb.Append("  EnableWetSign: ").Append(EnableWetSign).Append("\n");
            sb.Append("  AllowMarkup: ").Append(AllowMarkup).Append("\n");
            sb.Append("  AllowReassign: ").Append(AllowReassign).Append("\n");
            sb.Append("  CreatedDateTime: ").Append(CreatedDateTime).Append("\n");
            sb.Append("  LastModifiedDateTime: ").Append(LastModifiedDateTime).Append("\n");
            sb.Append("  DeliveredDateTime: ").Append(DeliveredDateTime).Append("\n");
            sb.Append("  SentDateTime: ").Append(SentDateTime).Append("\n");
            sb.Append("  CompletedDateTime: ").Append(CompletedDateTime).Append("\n");
            sb.Append("  VoidedDateTime: ").Append(VoidedDateTime).Append("\n");
            sb.Append("  VoidedReason: ").Append(VoidedReason).Append("\n");
            sb.Append("  DeletedDateTime: ").Append(DeletedDateTime).Append("\n");
            sb.Append("  DeclinedDateTime: ").Append(DeclinedDateTime).Append("\n");
            sb.Append("  StatusChangedDateTime: ").Append(StatusChangedDateTime).Append("\n");
            sb.Append("  DocumentsCombinedUri: ").Append(DocumentsCombinedUri).Append("\n");
            sb.Append("  CertificateUri: ").Append(CertificateUri).Append("\n");
            sb.Append("  TemplatesUri: ").Append(TemplatesUri).Append("\n");
            sb.Append("  MessageLock: ").Append(MessageLock).Append("\n");
            sb.Append("  RecipientsLock: ").Append(RecipientsLock).Append("\n");
            sb.Append("  BrandLock: ").Append(BrandLock).Append("\n");
            sb.Append("  BrandId: ").Append(BrandId).Append("\n");
            sb.Append("  UseDisclosure: ").Append(UseDisclosure).Append("\n");
            sb.Append("  EmailSettings: ").Append(EmailSettings).Append("\n");
            sb.Append("  PurgeState: ").Append(PurgeState).Append("\n");
            sb.Append("  LockInformation: ").Append(LockInformation).Append("\n");
            sb.Append("  Is21CFRPart11: ").Append(Is21CFRPart11).Append("\n");
            sb.Append("  IsUniversalSignatureEnvelope: ").Append(IsUniversalSignatureEnvelope).Append("\n");
            
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
            return this.Equals(obj as Envelope);
        }

        /// <summary>
        /// Returns true if Envelope instances are equal
        /// </summary>
        /// <param name="other">Instance of Envelope to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Envelope other)
        {
            // credit: http://stackoverflow.com/a/10454552/677735
            if (other == null)
                return false;

            return 
                (
                    this.TransactionId == other.TransactionId ||
                    this.TransactionId != null &&
                    this.TransactionId.Equals(other.TransactionId)
                ) && 
                (
                    this.Status == other.Status ||
                    this.Status != null &&
                    this.Status.Equals(other.Status)
                ) && 
                (
                    this.DocumentsUri == other.DocumentsUri ||
                    this.DocumentsUri != null &&
                    this.DocumentsUri.Equals(other.DocumentsUri)
                ) && 
                (
                    this.RecipientsUri == other.RecipientsUri ||
                    this.RecipientsUri != null &&
                    this.RecipientsUri.Equals(other.RecipientsUri)
                ) && 
                (
                    this.Asynchronous == other.Asynchronous ||
                    this.Asynchronous != null &&
                    this.Asynchronous.Equals(other.Asynchronous)
                ) && 
                (
                    this.EnvelopeUri == other.EnvelopeUri ||
                    this.EnvelopeUri != null &&
                    this.EnvelopeUri.Equals(other.EnvelopeUri)
                ) && 
                (
                    this.EmailSubject == other.EmailSubject ||
                    this.EmailSubject != null &&
                    this.EmailSubject.Equals(other.EmailSubject)
                ) && 
                (
                    this.EmailBlurb == other.EmailBlurb ||
                    this.EmailBlurb != null &&
                    this.EmailBlurb.Equals(other.EmailBlurb)
                ) && 
                (
                    this.EnvelopeId == other.EnvelopeId ||
                    this.EnvelopeId != null &&
                    this.EnvelopeId.Equals(other.EnvelopeId)
                ) && 
                (
                    this.SigningLocation == other.SigningLocation ||
                    this.SigningLocation != null &&
                    this.SigningLocation.Equals(other.SigningLocation)
                ) && 
                (
                    this.CustomFieldsUri == other.CustomFieldsUri ||
                    this.CustomFieldsUri != null &&
                    this.CustomFieldsUri.Equals(other.CustomFieldsUri)
                ) && 
                (
                    this.EnvelopeIdStamping == other.EnvelopeIdStamping ||
                    this.EnvelopeIdStamping != null &&
                    this.EnvelopeIdStamping.Equals(other.EnvelopeIdStamping)
                ) && 
                (
                    this.AuthoritativeCopy == other.AuthoritativeCopy ||
                    this.AuthoritativeCopy != null &&
                    this.AuthoritativeCopy.Equals(other.AuthoritativeCopy)
                ) && 
                (
                    this.Notification == other.Notification ||
                    this.Notification != null &&
                    this.Notification.Equals(other.Notification)
                ) && 
                (
                    this.NotificationUri == other.NotificationUri ||
                    this.NotificationUri != null &&
                    this.NotificationUri.Equals(other.NotificationUri)
                ) && 
                (
                    this.EnforceSignerVisibility == other.EnforceSignerVisibility ||
                    this.EnforceSignerVisibility != null &&
                    this.EnforceSignerVisibility.Equals(other.EnforceSignerVisibility)
                ) && 
                (
                    this.EnableWetSign == other.EnableWetSign ||
                    this.EnableWetSign != null &&
                    this.EnableWetSign.Equals(other.EnableWetSign)
                ) && 
                (
                    this.AllowMarkup == other.AllowMarkup ||
                    this.AllowMarkup != null &&
                    this.AllowMarkup.Equals(other.AllowMarkup)
                ) && 
                (
                    this.AllowReassign == other.AllowReassign ||
                    this.AllowReassign != null &&
                    this.AllowReassign.Equals(other.AllowReassign)
                ) && 
                (
                    this.CreatedDateTime == other.CreatedDateTime ||
                    this.CreatedDateTime != null &&
                    this.CreatedDateTime.Equals(other.CreatedDateTime)
                ) && 
                (
                    this.LastModifiedDateTime == other.LastModifiedDateTime ||
                    this.LastModifiedDateTime != null &&
                    this.LastModifiedDateTime.Equals(other.LastModifiedDateTime)
                ) && 
                (
                    this.DeliveredDateTime == other.DeliveredDateTime ||
                    this.DeliveredDateTime != null &&
                    this.DeliveredDateTime.Equals(other.DeliveredDateTime)
                ) && 
                (
                    this.SentDateTime == other.SentDateTime ||
                    this.SentDateTime != null &&
                    this.SentDateTime.Equals(other.SentDateTime)
                ) && 
                (
                    this.CompletedDateTime == other.CompletedDateTime ||
                    this.CompletedDateTime != null &&
                    this.CompletedDateTime.Equals(other.CompletedDateTime)
                ) && 
                (
                    this.VoidedDateTime == other.VoidedDateTime ||
                    this.VoidedDateTime != null &&
                    this.VoidedDateTime.Equals(other.VoidedDateTime)
                ) && 
                (
                    this.VoidedReason == other.VoidedReason ||
                    this.VoidedReason != null &&
                    this.VoidedReason.Equals(other.VoidedReason)
                ) && 
                (
                    this.DeletedDateTime == other.DeletedDateTime ||
                    this.DeletedDateTime != null &&
                    this.DeletedDateTime.Equals(other.DeletedDateTime)
                ) && 
                (
                    this.DeclinedDateTime == other.DeclinedDateTime ||
                    this.DeclinedDateTime != null &&
                    this.DeclinedDateTime.Equals(other.DeclinedDateTime)
                ) && 
                (
                    this.StatusChangedDateTime == other.StatusChangedDateTime ||
                    this.StatusChangedDateTime != null &&
                    this.StatusChangedDateTime.Equals(other.StatusChangedDateTime)
                ) && 
                (
                    this.DocumentsCombinedUri == other.DocumentsCombinedUri ||
                    this.DocumentsCombinedUri != null &&
                    this.DocumentsCombinedUri.Equals(other.DocumentsCombinedUri)
                ) && 
                (
                    this.CertificateUri == other.CertificateUri ||
                    this.CertificateUri != null &&
                    this.CertificateUri.Equals(other.CertificateUri)
                ) && 
                (
                    this.TemplatesUri == other.TemplatesUri ||
                    this.TemplatesUri != null &&
                    this.TemplatesUri.Equals(other.TemplatesUri)
                ) && 
                (
                    this.MessageLock == other.MessageLock ||
                    this.MessageLock != null &&
                    this.MessageLock.Equals(other.MessageLock)
                ) && 
                (
                    this.RecipientsLock == other.RecipientsLock ||
                    this.RecipientsLock != null &&
                    this.RecipientsLock.Equals(other.RecipientsLock)
                ) && 
                (
                    this.BrandLock == other.BrandLock ||
                    this.BrandLock != null &&
                    this.BrandLock.Equals(other.BrandLock)
                ) && 
                (
                    this.BrandId == other.BrandId ||
                    this.BrandId != null &&
                    this.BrandId.Equals(other.BrandId)
                ) && 
                (
                    this.UseDisclosure == other.UseDisclosure ||
                    this.UseDisclosure != null &&
                    this.UseDisclosure.Equals(other.UseDisclosure)
                ) && 
                (
                    this.EmailSettings == other.EmailSettings ||
                    this.EmailSettings != null &&
                    this.EmailSettings.Equals(other.EmailSettings)
                ) && 
                (
                    this.PurgeState == other.PurgeState ||
                    this.PurgeState != null &&
                    this.PurgeState.Equals(other.PurgeState)
                ) && 
                (
                    this.LockInformation == other.LockInformation ||
                    this.LockInformation != null &&
                    this.LockInformation.Equals(other.LockInformation)
                ) && 
                (
                    this.Is21CFRPart11 == other.Is21CFRPart11 ||
                    this.Is21CFRPart11 != null &&
                    this.Is21CFRPart11.Equals(other.Is21CFRPart11)
                ) && 
                (
                    this.IsUniversalSignatureEnvelope == other.IsUniversalSignatureEnvelope ||
                    this.IsUniversalSignatureEnvelope != null &&
                    this.IsUniversalSignatureEnvelope.Equals(other.IsUniversalSignatureEnvelope)
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
                
                if (this.TransactionId != null)
                    hash = hash * 59 + this.TransactionId.GetHashCode();
                
                if (this.Status != null)
                    hash = hash * 59 + this.Status.GetHashCode();
                
                if (this.DocumentsUri != null)
                    hash = hash * 59 + this.DocumentsUri.GetHashCode();
                
                if (this.RecipientsUri != null)
                    hash = hash * 59 + this.RecipientsUri.GetHashCode();
                
                if (this.Asynchronous != null)
                    hash = hash * 59 + this.Asynchronous.GetHashCode();
                
                if (this.EnvelopeUri != null)
                    hash = hash * 59 + this.EnvelopeUri.GetHashCode();
                
                if (this.EmailSubject != null)
                    hash = hash * 59 + this.EmailSubject.GetHashCode();
                
                if (this.EmailBlurb != null)
                    hash = hash * 59 + this.EmailBlurb.GetHashCode();
                
                if (this.EnvelopeId != null)
                    hash = hash * 59 + this.EnvelopeId.GetHashCode();
                
                if (this.SigningLocation != null)
                    hash = hash * 59 + this.SigningLocation.GetHashCode();
                
                if (this.CustomFieldsUri != null)
                    hash = hash * 59 + this.CustomFieldsUri.GetHashCode();
                
                if (this.EnvelopeIdStamping != null)
                    hash = hash * 59 + this.EnvelopeIdStamping.GetHashCode();
                
                if (this.AuthoritativeCopy != null)
                    hash = hash * 59 + this.AuthoritativeCopy.GetHashCode();
                
                if (this.Notification != null)
                    hash = hash * 59 + this.Notification.GetHashCode();
                
                if (this.NotificationUri != null)
                    hash = hash * 59 + this.NotificationUri.GetHashCode();
                
                if (this.EnforceSignerVisibility != null)
                    hash = hash * 59 + this.EnforceSignerVisibility.GetHashCode();
                
                if (this.EnableWetSign != null)
                    hash = hash * 59 + this.EnableWetSign.GetHashCode();
                
                if (this.AllowMarkup != null)
                    hash = hash * 59 + this.AllowMarkup.GetHashCode();
                
                if (this.AllowReassign != null)
                    hash = hash * 59 + this.AllowReassign.GetHashCode();
                
                if (this.CreatedDateTime != null)
                    hash = hash * 59 + this.CreatedDateTime.GetHashCode();
                
                if (this.LastModifiedDateTime != null)
                    hash = hash * 59 + this.LastModifiedDateTime.GetHashCode();
                
                if (this.DeliveredDateTime != null)
                    hash = hash * 59 + this.DeliveredDateTime.GetHashCode();
                
                if (this.SentDateTime != null)
                    hash = hash * 59 + this.SentDateTime.GetHashCode();
                
                if (this.CompletedDateTime != null)
                    hash = hash * 59 + this.CompletedDateTime.GetHashCode();
                
                if (this.VoidedDateTime != null)
                    hash = hash * 59 + this.VoidedDateTime.GetHashCode();
                
                if (this.VoidedReason != null)
                    hash = hash * 59 + this.VoidedReason.GetHashCode();
                
                if (this.DeletedDateTime != null)
                    hash = hash * 59 + this.DeletedDateTime.GetHashCode();
                
                if (this.DeclinedDateTime != null)
                    hash = hash * 59 + this.DeclinedDateTime.GetHashCode();
                
                if (this.StatusChangedDateTime != null)
                    hash = hash * 59 + this.StatusChangedDateTime.GetHashCode();
                
                if (this.DocumentsCombinedUri != null)
                    hash = hash * 59 + this.DocumentsCombinedUri.GetHashCode();
                
                if (this.CertificateUri != null)
                    hash = hash * 59 + this.CertificateUri.GetHashCode();
                
                if (this.TemplatesUri != null)
                    hash = hash * 59 + this.TemplatesUri.GetHashCode();
                
                if (this.MessageLock != null)
                    hash = hash * 59 + this.MessageLock.GetHashCode();
                
                if (this.RecipientsLock != null)
                    hash = hash * 59 + this.RecipientsLock.GetHashCode();
                
                if (this.BrandLock != null)
                    hash = hash * 59 + this.BrandLock.GetHashCode();
                
                if (this.BrandId != null)
                    hash = hash * 59 + this.BrandId.GetHashCode();
                
                if (this.UseDisclosure != null)
                    hash = hash * 59 + this.UseDisclosure.GetHashCode();
                
                if (this.EmailSettings != null)
                    hash = hash * 59 + this.EmailSettings.GetHashCode();
                
                if (this.PurgeState != null)
                    hash = hash * 59 + this.PurgeState.GetHashCode();
                
                if (this.LockInformation != null)
                    hash = hash * 59 + this.LockInformation.GetHashCode();
                
                if (this.Is21CFRPart11 != null)
                    hash = hash * 59 + this.Is21CFRPart11.GetHashCode();
                
                if (this.IsUniversalSignatureEnvelope != null)
                    hash = hash * 59 + this.IsUniversalSignatureEnvelope.GetHashCode();
                
                return hash;
            }
        }

    }
}
