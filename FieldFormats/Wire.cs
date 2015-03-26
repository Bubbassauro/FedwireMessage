using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using FedwireMessage.FieldFormats;
using System.Text.RegularExpressions;

namespace FedwireMessage.FieldFormats
{
    /// <summary>
    /// Formatting rules according to https://www.frbservices.org/campaigns/remittance/files/fedwire_funds_format_reference_guide.pdf
    /// </summary>
    public class Wire
    {
        private string _wireLine;
        private Tags _tags;

        public Wire(string wireLine)
        {
            _wireLine = wireLine;
            _tags = new Tags();

            var regex = new Regex(@"(\{[^\{\}]+\})([^\{\}]+)");

            foreach (Match match in regex.Matches(_wireLine))
            {
                _tags.Add(match.Groups[1].Value.Trim(), match.Groups[2].Value.Trim());
            }
        }

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
        public string SenderSuppliedInformation { get { return _tags["{1500}"]; } }

        [Display(Name = "Type/Subtype")]
        public TypeSubtype TypeSubtype { get { return new TypeSubtype(_tags["{1510}"]); } }

        [Display(Name = "Input Message Accountability Data (IMAD)")]
        public IMAD IMAD
        {
            get
            {
                return new IMAD(_tags["{1520}"]);
            }
        }

        /// <summary>
        /// Format: 12 numeric, right-justified with leading zeros, an implied
        /// decimal point and no commas; e.g., $12,345.67 becomes
        /// 000001234567
        /// </summary>
        [Display(Name = "Amount (up to a penny less than $10 billion)")]
        [DisplayFormat(DataFormatString = "{0:N}")]
        public decimal Amount
        {
            get
            {
                decimal amount;
                if (decimal.TryParse(_tags["{2000}"], out amount))
                    return amount / 100;
                else
                    return 0;
            }
        }

        [Display(Name = "Sender DI")]
        public DI SenderDI { get { return new DI(_tags["{3100}"]); } }

        [Display(Name = "Receiver DI")]
        public DI ReceiverDI { get { return new DI(_tags["{3400}"]); } }

        /// <summary>
        /// Business Function Code (3 characters)
        /// BTR, CKS, CTP, CTR, DEP, DRB, DRC, DRW, FFR, FFS, SVC
        /// Transaction Type Code (3 characters)
        /// </summary>
        [Display(Name = "Business Function Code")]
        public BusinessFunction BusinessFunction { get { return new BusinessFunction(_tags["{3600}"]); } }

        // Other transfer information
        [Display(Name = "Sender Reference")]
        public string SenderReference { get { return _tags["{3320}"]; } }

        [Display(Name = "Previous Message Identifier")]
        public string PreviousMessageIdentifier { get { return _tags["{3500}"]; } }

        /// <summary>
        /// Local Instrument Code (4 character code)
        /// ANSI, COVS, GXML, IXML, NARR, PROP, RMTS, RRMT,
        /// S820, SWIF, UEDI
        /// Proprietary Code (35 characters)
        /// </summary>
        [Display(Name = "Local Instrument")]
        public string LocalInstrument { get { return _tags["{3610}"]; } }

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
        public string PaymentNotification { get { return _tags["{3620}"]; } }

        [Display(Name = "Charges")]
        public string Charges { get { return _tags["{3700}"]; } }

        /// <summary>
        /// Currency Code (3 characters)
        /// Amount (15 characters)
        /// </summary>
        [Display(Name = "Instructed Amount")]
        public string InstructedAmount { get { return _tags["{3710}"]; } }

        [Display(Name = "Exchange Rate")]
        public string ExchangeRate { get { return _tags["{3720}"]; } }

        // Beneficiary Information
        /// <summary>
        /// ID Code (B, C, D, F, U)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Intermediary FI")]
        public Information IntermediaryFI { get { return new Information(_tags["{4000}"]); } }

        /// <summary>
        /// ID Code (B, C, D, F, U)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Beneficiary FI")]
        public Information BeneficiaryFI { get { return new Information(_tags["{4100}"]); } }

        /// <summary>
        /// ID Code (B, C, D, F, T, U, 1, 2, 3, 4, 5, 9)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters  each)
        /// </summary>
        [Display(Name = "Beneficiary")]
        public Information Beneficiary { get { return new Information(_tags["{4200}"]); } }

        [Display(Name = "Reference for Beneficiary")]
        public string ReferenceForBeneficiary { get { return _tags["{4320}"]; } }

        /// <summary>
        /// ID Code (D)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Account Debited in Drawdown")]
        public Information AccountDebitedInDrawdown { get { return new Information(_tags["{4400}"]); } }

        // Originator Information 
        /// <summary>
        /// ID Code (B, C, D, F, T, U, 1, 2, 3, 4, 5, 9)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Originator")]
        public Information Originator { get { return new Information(_tags["{5000}"]); } }

        /// <summary>
        /// Party Identifier (35 characters)
        /// </summary>
        [Display(Name = "Originator Option F")]
        public string OriginatorOptionF { get { return _tags["{5010}"]; } }

        /// <summary>
        /// ID Code (B, C, D, F, U)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Originator FI")]
        public Information OriginatorFI { get { return new Information(_tags["{5100}"]); } }

        /// <summary>
        /// ID Code (B, C, D, F, U)
        /// Identifier (34 characters)
        /// Name (35 characters)
        /// Address (3 lines of 35 characters each)
        /// </summary>
        [Display(Name = "Instructing FI")]
        public Information InstructingFI { get { return new Information(_tags["{5200}"]); } }

        /// <summary>
        /// Drawdown Credit Account Number (9 character ABA)
        /// </summary>
        [Display(Name = "Account Credited in Drawdown")]
        public string AccountCreditedInDrawdown { get { return _tags["{5400}"]; } }

        [Display(Name = "Originator to Beneficiary Information")]
        public string[] OriginatorToBeneficiaryInformation
        {
            get
            {
                string[] lines = _tags["{6000}"].Split('*');
                return lines;
            }
        }

        // Financial Institution Information 
        [Display(Name = "Receiver FI Information")]
        public string ReceiverFI { get { return _tags["{6100}"]; } }

        /// <summary>
        /// Advice Code (LTR, PHN, TLX or WRE)
        /// Additional Information (1 line of 26 characters, plus up to 5
        /// lines of 33 characters each)
        /// </summary>
        [Display(Name = "Drawdown Debit Account Advice Information")]
        public string DrawdownDebitAccountAdviceInformation { get { return _tags["{6110}"]; } }

        [Display(Name = "Intermediary FI Information")]
        public string IntermediaryFiInformation { get { return _tags["{6200}"]; } }

        /// <summary>
        /// Advice Code (LTR, PHN, TLX or WRE)
        /// Additional Information (1 line of 26 characters, plus up to 5
        /// lines of 33 characters each)
        /// </summary>
        [Display(Name = "Intermediary FI Advice Information")]
        public string IntermediaryFIAdviceInformation { get { return _tags["{6210}"]; } }

        [Display(Name = "Beneficiary’s FI Information")]
        public string BeneficiaryFiInformation { get { return _tags["{6300}"]; } }

        /// <summary>
        /// Advice Code (LTR, PHN, TLX or WRE)
        /// Additional Information (1 line of 26 characters, plus up to 5
        /// lines of 33 characters each)
        /// </summary>
        [Display(Name = "Beneficiary’s FI Advice Information")]
        public string BeneficiaryFiAdviceInformation { get { return _tags["{6310}"]; } }

        [Display(Name = "Beneficiary Information")]
        public string BeneficiaryInformation { get { return _tags["{6400}"]; } }

        /// <summary>
        /// Advice Code (LTR, PHN, TLX, WRE or HLD)
        /// Additional Information (1 line of 26 characters, plus up to 5 lines of 33 characters each)
        /// </summary>
        [Display(Name = "Beneficiary Advice Information")]
        public string BeneficiaryAdviceInformation { get { return _tags["{6410}"]; } }

        [Display(Name = "Method of Payment to Beneficiary")]
        public string MethodOfPaymentToBeneficiary { get { return _tags["{6420}"]; } }

        [Display(Name = "FI to FI Information")]
        public string FiToFi { get { return _tags["{6500}"]; } }

        // Cover Payment Information
        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// Instructed Amount (18 characters)
        /// </summary>
        [Display(Name = "Sequence B 33B Currency/Instructed Amount")]
        public string SequenceB_33B_CurrencyInstructedAmount { get { return _tags["{7033}"]; } }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 50a Ordering Customer")]
        public string SequenceB_50a_OrderingCustomer { get { return _tags["{7050}"]; } }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 52a Ordering Institution")]
        public string SequenceB_52a_OrderingInstitution { get { return _tags["{7052}"]; } }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 56a Intermediary Institution")]
        public string SequenceB_56a_IntermediaryInstitution { get { return _tags["{7056}"]; } }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 57a Account with Institution")]
        public string SequenceB_57a_AccountWithInstitution { get { return _tags["{7057}"]; } }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 59a Beneficiary Customer")]
        public string SequenceB_59a_BeneficiaryCustomer { get { return _tags["{7059}"]; } }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 70 Remittance Information")]
        public string SequenceB_70_RemittanceInformation { get { return _tags["{7070}"]; } }

        /// <summary>
        /// SWIFT Field Tag (5 characters)
        /// </summary>
        [Display(Name = "Sequence B 72 Sender to Receiver Information")]
        public string SequenceB_72_SenderToReceiverInformation { get { return _tags["{7072}"]; } }

        ///Addenda Length (4 characters)
        /// Addenda Information (8,994 characters)
        // Unstructured Addenda Information
        [Display(Name = "Unstructured Addenda Information")]
        public string UnstructuredAddendaInformation { get { return _tags["{8200}"]; } }

        // Related Remittance Information
        [Display(Name = "Related Remittance Information")]
        public string RelatedRemittanceInformation { get { return _tags["{8250}"]; } }

        // Structured Remittance Information

        [Display(Name = "Remittance Originator")]
        public string RemittanceOriginator { get { return _tags["{8300}"]; } }

        [Display(Name = "Remittance Beneficiary")]
        public string RemittanceBeneficiary { get { return _tags["{8350}"]; } }

        [Display(Name = "Primary Remittance Document Information")]
        public string PrimaryRemittanceDocumentInformation { get { return _tags["{8400}"]; } }

        [Display(Name = "Actual Amount Paid")]
        public string ActualAmountPaid { get { return _tags["{8450}"]; } }

        /// <summary>
        /// Currency Code (3 characters)
        /// Amount (19 characters) 
        /// </summary>
        [Display(Name = "Gross Amount of Remittance Document")]
        public string GrossAmountOfRemittanceDocument { get { return _tags["{8500}"]; } }

        /// <summary>
        /// Currency Code (3 characters)
        /// Amount (19 characters) 
        /// </summary>
        [Display(Name = "Amount of Negotiated Discount")]
        public string AmountOfNegotiatedDiscount { get { return _tags["{8550}"]; } }

        [Display(Name = "Adjustment Information")]
        public string AdjustmentInformation { get { return _tags["{8600}"]; } }

        [Display(Name = "Date of Remittance Document (CCYYMMDD format) ")]
        public string DateOfRemittanceDocument { get { return _tags["{8650}"]; } }

        [Display(Name = "Secondary Remittance Document Information")]
        public string SecondaryRemittanceDocumentInformation { get { return _tags["{8700}"]; } }

        [Display(Name = "Remittance Free Text")]
        public string RemittanceFreeText { get { return _tags["{8750}"]; } }

        // Service Message Information
        [Display(Name = "Service Message Information")]
        public string ServiceMessageInformation { get { return _tags["{9000}"]; } }

        // Information Appended by the Fedwire Funds Service

        [Display(Name = "Message Disposition")]
        public string MessageDisposition { get { return _tags["{1100}"]; } }

        [Display(Name = "Receipt Time Stamp")]
        public string ReceiptTimeStamp { get { return _tags["{1110}"]; } }

        /// <summary>
        /// Output Cycle Date (CCYYMMDD)
        /// Output Destination ID (8 characters)
        /// Output Sequence Number (6 characters)
        /// Output Date (MMDD, based on the calendar date)
        /// Output Time (HHMM, based on a 24-hour clock, Eastern Time)
        /// Output FRB Application ID (4 characters)
        /// </summary>
        [Display(Name = "Output Message Accountability Data (OMAD)")]
        public string OMAD { get { return _tags["{1120}"]; } }

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
        public string Error { get { return _tags["{1130}"]; } }

        #endregion
    }
}
