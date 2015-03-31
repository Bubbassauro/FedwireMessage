using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using FedwireMessage.FieldFormats;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace FedwireMessage.FieldFormats
{
    /// <summary>
    /// Formatting rules according to https://www.frbservices.org/campaigns/remittance/files/fedwire_funds_format_reference_guide.pdf
    /// </summary>
    public class Wire
    {
        private string _wireLine;
        private Tags _tags;

        public Wire() { }

        public Wire(string wireLine)
        {
            _wireLine = wireLine;
            _tags = new Tags();

            var regex = new Regex(@"(\{[^\{\}]+\})([^\{\}]+)");

            // Read tags from message
            foreach (Match match in regex.Matches(_wireLine))
            {
                _tags.Add(match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim());
            }

            // Load tags into properties
            SenderSuppliedInformation = _tags["{1500}"];
            TypeSubtype = new TypeSubtype(_tags["{1510}"]);
            IMAD = new IMAD(_tags["{1520}"]);

            decimal amount = 0;
            if (decimal.TryParse(_tags["{2000}"], out amount))
                Amount = amount / 100;

            SenderDI = new DI(_tags["{3100}"]);
            ReceiverDI = new DI(_tags["{3400}"]);
            BusinessFunction = new BusinessFunction(_tags["{3600}"]);
            SenderReference = _tags["{3320}"];
            PreviousMessageIdentifier = _tags["{3500}"];
            LocalInstrument = _tags["{3610}"];
            PaymentNotification = _tags["{3620}"];
            Charges = _tags["{3700}"];
            InstructedAmount = _tags["{3710}"];
            ExchangeRate = _tags["{3720}"];
            IntermediaryFI = new Information(_tags["{4000}"]);
            BeneficiaryFI = new Information(_tags["{4100}"]);
            Beneficiary = new Information(_tags["{4200}"]);
            ReferenceForBeneficiary = _tags["{4320}"];
            AccountDebitedInDrawdown = new Information(_tags["{4400}"]);
            Originator = new Information(_tags["{5000}"]);
            OriginatorOptionF = _tags["{5010}"];
            OriginatorFI = new Information(_tags["{5100}"]);
            InstructingFI = new Information(_tags["{5200}"]);
            AccountCreditedInDrawdown = new Information(_tags["{5400}"]);
            OriginatorToBeneficiaryInformation = _tags["{6000}"].Split('*');
            ReceiverFI = _tags["{6100}"];
            DrawdownDebitAccountAdviceInformation = _tags["{6110}"];
            IntermediaryFiInformation = _tags["{6200}"];
            IntermediaryFIAdviceInformation = _tags["{6210}"];
            BeneficiaryFiInformation = _tags["{6300}"];
            BeneficiaryFiAdviceInformation = _tags["{6310}"];
            BeneficiaryInformation = _tags["{6400}"];
            BeneficiaryAdviceInformation = _tags["{6410}"];
            MethodOfPaymentToBeneficiary = _tags["{6420}"];
            FiToFi = _tags["{6500}"];
            SequenceB_33B_CurrencyInstructedAmount = _tags["{7033}"];
            SequenceB_50a_OrderingCustomer = _tags["{7050}"];
            SequenceB_52a_OrderingInstitution = _tags["{7052}"];
            SequenceB_56a_IntermediaryInstitution = _tags["{7056}"];
            SequenceB_57a_AccountWithInstitution = _tags["{7057}"];
            SequenceB_59a_BeneficiaryCustomer = _tags["{7059}"];
            SequenceB_70_RemittanceInformation = _tags["{7070}"];
            SequenceB_72_SenderToReceiverInformation = _tags["{7072}"];
            UnstructuredAddendaInformation = _tags["{8200}"];
            RelatedRemittanceInformation = _tags["{8250}"];
            RemittanceOriginator = _tags["{8300}"];
            RemittanceBeneficiary = _tags["{8350}"];
            PrimaryRemittanceDocumentInformation = _tags["{8400}"];
            ActualAmountPaid = _tags["{8450}"];
            GrossAmountOfRemittanceDocument = _tags["{8500}"];
            AmountOfNegotiatedDiscount = _tags["{8550}"];
            AdjustmentInformation = _tags["{8600}"];
            DateOfRemittanceDocument = _tags["{8650}"];
            SecondaryRemittanceDocumentInformation = _tags["{8700}"];
            RemittanceFreeText = _tags["{8750}"];
            ServiceMessageInformation = _tags["{9000}"];
            MessageDisposition = _tags["{1100}"];
            ReceiptTimeStamp = _tags["{1110}"];
            OMAD = _tags["{1120}"];
            Error = _tags["{1130}"];

            // Load Frb code lists
            BusinessFunctions = JsonConvert.DeserializeObject<Tags>(FrbCodes.Resources.BusinessFunctionCode);
            TypeCodes = JsonConvert.DeserializeObject<Tags>(FrbCodes.Resources.TypeCode);
            SubTypeCodes = JsonConvert.DeserializeObject<Tags>(FrbCodes.Resources.SubTypeCode);
            IdCodes = JsonConvert.DeserializeObject<Tags>(FrbCodes.Resources.IdCodes);

            // Add code descriptions
            BusinessFunction.BusinessFunctionDescription = BusinessFunctions[BusinessFunction.BusinessFunctionCode];
            TypeSubtype.TypeDescription = TypeCodes[TypeSubtype.TypeCode];
            TypeSubtype.SubtypeDescription = SubTypeCodes[TypeSubtype.SubtypeCode];

            IntermediaryFI.IdCodeDescription = IdCodes[IntermediaryFI.IdCode];
            BeneficiaryFI.IdCodeDescription = IdCodes[BeneficiaryFI.IdCode];
            Beneficiary.IdCodeDescription = IdCodes[Beneficiary.IdCode];
            AccountDebitedInDrawdown.IdCodeDescription = IdCodes[AccountDebitedInDrawdown.IdCode];
            Originator.IdCodeDescription = IdCodes[Originator.IdCode];
            OriginatorFI.IdCodeDescription = IdCodes[OriginatorFI.IdCode];
            InstructingFI.IdCodeDescription = IdCodes[InstructingFI.IdCode];
        }

        public Tags BusinessFunctions { get; set; }
        public Tags SubTypeCodes { get; set; }
        public Tags TypeCodes { get; set; }
        public Tags IdCodes { get; set; }
        public List<Wire> Wires { get; set; }

        public Tags Tags
        {
            get { return _tags; }
        }

        public string WireLine
        {
            get { return _wireLine; }
            set { _wireLine = value; }
        }

        #region Properties

        /// <summary>
        /// Format Version (‘30’)
        /// User Request Correlation (8 characters)
        /// Test Production Code (‘T’ test or ‘P’ production)
        /// Message Duplication Code (‘ ‘ original message or ‘P’ resend)
        /// </summary>
        [Display(Name = "Sender Supplied Information")]
        public string SenderSuppliedInformation { get; set; }

        [Display(Name = "Type/Subtype")]
        public TypeSubtype TypeSubtype { get; set; }

        [Display(Name = "Input Message Accountability Data (IMAD)")]
        public IMAD IMAD { get; set; }

        /// <summary>
        /// Format: 12 numeric, right-justified with leading zeros, an implied
        /// decimal point and no commas; e.g., $12,345.67 becomes
        /// 000001234567
        /// </summary>
        [Display(Name = "Amount (up to a penny less than $10 billion)")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public decimal Amount { get; set; }

        [Display(Name = "Sender DI")]
        public DI SenderDI { get; set; }

        [Display(Name = "Receiver DI")]
        public DI ReceiverDI { get; set; }

        /// <summary>
        /// Business Function Code (3 characters)
        /// BTR, CKS, CTP, CTR, DEP, DRB, DRC, DRW, FFR, FFS, SVC
        /// Transaction Type Code (3 characters)
        /// </summary>
        [Display(Name = "Business Function Code")]
        public BusinessFunction BusinessFunction { get; set; }

        // Other transfer information
        [Display(Name = "Sender Reference")]
        public string SenderReference { get; set; }

        [Display(Name = "Previous Message Identifier")]
        public string PreviousMessageIdentifier { get; set; }

        /// <summary>
        /// Local Instrument Code (4 character code)
        /// ANSI, COVS, GXML, IXML, NARR, PROP, RMTS, RRMT,
        /// S820, SWIF, UEDI
        /// Proprietary Code (35 characters)
        /// </summary>
        [Display(Name = "Local Instrument")]
        public string LocalInstrument { get; set; }

        /// <summary>
        /// Payment Notification Indicator (‘0’ through ‘9’)
        /// Contact Notification Electronic Address (2,048 characters;
        /// i.e., E-mail or URL address)
        /// Contact Name (140 characters)
        /// Contact Phone Number (35 characters)
        /// Contact Mobile Number (35 characters)
        /// Contact Fax Number (35 characters)
        /// End-to-End Identification (35 characters)
        /// </summary>
        [Display(Name = "Payment Notification")]
        public string PaymentNotification { get; set; }

        [Display(Name = "Charges")]
        public string Charges { get; set; }

        /// <summary>
        /// Currency Code (3 characters)
        /// Amount (15 characters)
        /// </summary>
        [Display(Name = "Instructed Amount")]
        public string InstructedAmount { get; set; }

        [Display(Name = "Exchange Rate")]
        public string ExchangeRate { get; set; }

        // Beneficiary Information
        /// <summary>
        /// ID Code (B, C, D, F, U)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Intermediary FI")]
        public Information IntermediaryFI { get; set; }

        /// <summary>
        /// ID Code (B, C, D, F, U)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Beneficiary FI")]
        public Information BeneficiaryFI { get; set; }

        /// <summary>
        /// ID Code (B, C, D, F, T, U, 1, 2, 3, 4, 5, 9)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters  each)
        /// </summary>
        [Display(Name = "Beneficiary")]
        public Information Beneficiary { get; set; }

        [Display(Name = "Reference for Beneficiary")]
        public string ReferenceForBeneficiary { get; set; }

        /// <summary>
        /// ID Code (D)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Account Debited in Drawdown")]
        public Information AccountDebitedInDrawdown { get; set; }

        // Originator Information 
        /// <summary>
        /// ID Code (B, C, D, F, T, U, 1, 2, 3, 4, 5, 9)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Originator")]
        public Information Originator { get; set; }

        /// <summary>
        /// Party Identifier (35 characters)
        /// </summary>
        [Display(Name = "Originator Option F")]
        public string OriginatorOptionF { get; set; }

        /// <summary>
        /// ID Code (B, C, D, F, U)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Originator FI")]
        public Information OriginatorFI { get; set; }

        /// <summary>
        /// ID Code (B, C, D, F, U)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Instructing FI")]
        public Information InstructingFI { get; set; }

        /// <summary>
        /// Drawdown Credit Account Number (9 character ABA)
        /// </summary>
        [Display(Name = "Account Credited in Drawdown")]
        public Information AccountCreditedInDrawdown { get; set; }

        [Display(Name = "Originator to Beneficiary Information")]
        public string[] OriginatorToBeneficiaryInformation { get; set; }

        // Financial Institution Information 
        [Display(Name = "Receiver FI Information")]
        public string ReceiverFI { get; set; }

        /// <summary>
        /// Advice Code (LTR, PHN, TLX or WRE)
        /// Additional Information (1 line of 26 characters, plus up to 5
        /// lines of 33 characters each)
        /// </summary>
        [Display(Name = "Drawdown Debit Account Advice Information")]
        public string DrawdownDebitAccountAdviceInformation { get; set; }

        [Display(Name = "Intermediary FI Information")]
        public string IntermediaryFiInformation { get; set; }

        /// <summary>
        /// Advice Code (LTR, PHN, TLX or WRE)
        /// Additional Information (1 line of 26 characters, plus up to 5
        /// lines of 33 characters each)
        /// </summary>
        [Display(Name = "Intermediary FI Advice Information")]
        public string IntermediaryFIAdviceInformation { get; set; }

        [Display(Name = "Beneficiary’s FI Information")]
        public string BeneficiaryFiInformation { get; set; }

        /// <summary>
        /// Advice Code (LTR, PHN, TLX or WRE)
        /// Additional Information (1 line of 26 characters, plus up to 5
        /// lines of 33 characters each)
        /// </summary>
        [Display(Name = "Beneficiary’s FI Advice Information")]
        public string BeneficiaryFiAdviceInformation { get; set; }

        [Display(Name = "Beneficiary Information")]
        public string BeneficiaryInformation { get; set; }

        /// <summary>
        /// Advice Code (LTR, PHN, TLX, WRE or HLD)
        /// Additional Information (1 line of 26 characters, plus up to 5 lines of 33 characters each)
        /// </summary>
        [Display(Name = "Beneficiary Advice Information")]
        public string BeneficiaryAdviceInformation { get; set; }

        [Display(Name = "Method of Payment to Beneficiary")]
        public string MethodOfPaymentToBeneficiary { get; set; }

        [Display(Name = "FI to FI Information")]
        public string FiToFi { get; set; }

        // Cover Payment Information
        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// Instructed Amount (18 characters)
        /// </summary>
        [Display(Name = "Sequence B 33B Currency/Instructed Amount")]
        public string SequenceB_33B_CurrencyInstructedAmount { get; set; }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 50a Ordering Customer")]
        public string SequenceB_50a_OrderingCustomer { get; set; }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 52a Ordering Institution")]
        public string SequenceB_52a_OrderingInstitution { get; set; }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 56a Intermediary Institution")]
        public string SequenceB_56a_IntermediaryInstitution { get; set; }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 57a Account with Institution")]
        public string SequenceB_57a_AccountWithInstitution { get; set; }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 59a Beneficiary Customer")]
        public string SequenceB_59a_BeneficiaryCustomer { get; set; }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 70 Remittance Information")]
        public string SequenceB_70_RemittanceInformation { get; set; }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 72 Sender to Receiver Information")]
        public string SequenceB_72_SenderToReceiverInformation { get; set; }

        ///Addenda Length (4 characters)
        /// Addenda Information (8,994 characters)
        // Unstructured Addenda Information
        [Display(Name = "Unstructured Addenda Information")]
        public string UnstructuredAddendaInformation { get; set; }

        // Related Remittance Information
        [Display(Name = "Related Remittance Information")]
        public string RelatedRemittanceInformation { get; set; }

        // Structured Remittance Information

        [Display(Name = "Remittance Originator")]
        public string RemittanceOriginator { get; set; }

        [Display(Name = "Remittance Beneficiary")]
        public string RemittanceBeneficiary { get; set; }

        [Display(Name = "Primary Remittance Document Information")]
        public string PrimaryRemittanceDocumentInformation { get; set; }

        [Display(Name = "Actual Amount Paid")]
        public string ActualAmountPaid { get; set; }

        /// <summary>
        /// Currency Code (3 characters)
        /// Amount (19 characters) 
        /// </summary>
        [Display(Name = "Gross Amount of Remittance Document")]
        public string GrossAmountOfRemittanceDocument { get; set; }

        /// <summary>
        /// Currency Code (3 characters)
        /// Amount (19 characters) 
        /// </summary>
        [Display(Name = "Amount of Negotiated Discount")]
        public string AmountOfNegotiatedDiscount { get; set; }

        [Display(Name = "Adjustment Information")]
        public string AdjustmentInformation { get; set; }

        [Display(Name = "Date of Remittance Document (CCYYMMDD format) ")]
        public string DateOfRemittanceDocument { get; set; }

        [Display(Name = "Secondary Remittance Document Information")]
        public string SecondaryRemittanceDocumentInformation { get; set; }

        [Display(Name = "Remittance Free Text")]
        public string RemittanceFreeText { get; set; }

        // Service Message Information
        [Display(Name = "Service Message Information")]
        public string ServiceMessageInformation { get; set; }

        // Information Appended by the Fedwire Funds Service

        [Display(Name = "Message Disposition")]
        public string MessageDisposition { get; set; }

        [Display(Name = "Receipt Time Stamp")]
        public string ReceiptTimeStamp { get; set; }

        /// <summary>
        /// Output Cycle Date (CCYYMMDD)
        /// Output Destination ID (8 characters)
        /// Output Sequence Number (6 characters)
        /// Output Date (MMDD, based on the calendar date)
        /// Output Time (HHMM, based on a 24-hour clock, Eastern Time)
        /// Output FRB Application ID (4 characters)
        /// </summary>
        [Display(Name = "Output Message Accountability Data (OMAD)")]
        public string OMAD { get; set; }

        /// <summary>
        /// Error Category (1 character code)
        /// E Data Error H Accountability Error
        ///  F Insufficient Balance W Cutoff Hour Error
        ///  X Duplicate IMAD I In Process or Intercepted
        /// 
        /// Error Code (3 characters)
        /// Error Description (35 characters)
        /// </summary>
        [Display(Name = "Error")]
        public string Error { get; set; }

        #endregion
    }
}
