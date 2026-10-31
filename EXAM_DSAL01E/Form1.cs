using System;
using System.Globalization;
using System.Windows.Forms;

namespace EXAM_DSAL01E
{
    public partial class Form1 : Form
    {
        const decimal RATE = 23.5391m;   // rate per cu.m
        const decimal ENV = 108.28m;   // Environmental Charges (fixed)
        const decimal MSC = 1.53m;    // Maintenance Service Charge
        const decimal TAX_RATE = 0.025m;   // 2.5% Government Taxes
        static readonly CultureInfo CI = CultureInfo.InvariantCulture;

        public Form1()
        {
            InitializeComponent();

            // Auto-compute while typing the readings
            txtPresent.TextChanged += (s, e) => Calculate();
            txtPrevious.TextChanged += (s, e) => Calculate();

            // Computed fields: user cannot type in them
            txtConsumption.ReadOnly = txtCurrent.ReadOnly = txtBasic.ReadOnly =
            txtEnv.ReadOnly = txtMSC.ReadOnly = txtBefore.ReadOnly =
            txtTax.ReadOnly = txtTotal.ReadOnly = true;
        }

        private void Form1_Load(object sender, EventArgs e) { }

        // ============ COMPUTATION ============
        bool Calculate()
        {
            int present, previous;
            bool ok1 = int.TryParse(txtPresent.Text, out present);
            bool ok2 = int.TryParse(txtPrevious.Text, out previous);

            if (!ok1 || !ok2 || present < previous)
            {
                txtConsumption.Text = txtCurrent.Text = txtBasic.Text = txtEnv.Text =
                txtMSC.Text = txtBefore.Text = txtTax.Text = txtTotal.Text = "";
                return false;
            }

            int consumption = present - previous;
            decimal basic = Math.Round(RATE * consumption, 2, MidpointRounding.AwayFromZero);
            decimal env = ENV;
            decimal before = basic + env + MSC;
            decimal tax = Math.Round(before * TAX_RATE, 2, MidpointRounding.AwayFromZero);
            decimal total = before + tax;

            txtConsumption.Text = consumption.ToString();
            txtBasic.Text = basic.ToString("N2", CI);
            txtEnv.Text = env.ToString("N2", CI);
            txtMSC.Text = MSC.ToString("N2", CI);
            txtBefore.Text = before.ToString("N2", CI);
            txtTax.Text = tax.ToString("N2", CI);
            txtTotal.Text = total.ToString("N2", CI);
            txtCurrent.Text = total.ToString("N2", CI);
            return true;
        }

        // ============ BUILD DATA FOR RECEIPT ============
        ReceiptData GetData()
        {
            if (!Calculate())
            {
                MessageBox.Show("Enter valid whole-number readings.\nPresent Reading must not be less than Previous Reading.",
                                "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }

            ReceiptData d = new ReceiptData();
            d.Contract = txtContract.Text;
            d.Name = txtAccountName.Text;
            d.Address = txtAddress.Text;
            d.TIN = txtTIN.Text;
            d.RateClass = txtRateClass.Text;
            d.Business = txtBusiness.Text;
            d.MeterNo = txtMeterNo.Text;
            d.MRU = txtMRU.Text;
            d.Seq = txtSeq.Text;
            d.ReadingDate = txtReadingDate.Text;
            d.Present = txtPresent.Text;
            d.Previous = txtPrevious.Text;
            d.Consumption = txtConsumption.Text;

            DateTime rd;
            d.MonthYear = DateTime.TryParse(txtReadingDate.Text, out rd)
                          ? rd.ToString("MMMM yyyy", CI) : "";

            // Billing period: start typed by user, last date = Reading Date
            d.Period = txtBillingPeriod.Text + " to " + txtReadingDate.Text;

            d.Basic = txtBasic.Text;
            d.Env = txtEnv.Text;
            d.MSC = txtMSC.Text;
            d.Before = txtBefore.Text;
            d.Tax = txtTax.Text;
            d.Total = txtTotal.Text;
            d.DueDate = txtDueDate.Text;
            return d;
        }

        static string LR(string left, string right, int width = 46)
        {
            int pad = width - left.Length - right.Length;
            if (pad < 1) pad = 1;
            return left + new string(' ', pad) + right;
        }

        // ============ BUTTONS ============
        // CALCULATE
        private void button1_Click(object sender, EventArgs e)
        {
            if (!Calculate())
                MessageBox.Show("Enter valid whole-number readings.\nPresent Reading must not be less than Previous Reading.",
                                "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // PREVIEW RECEIPT
        private void button2_Click(object sender, EventArgs e)
        {
            ReceiptData d = GetData();
            if (d == null) return;

            lstReceipt.Items.Clear();
            lstReceipt.Items.Add("Maynilad Water Services, Inc.");
            lstReceipt.Items.Add("8398 DR A SANTOS AVE BF HOMES");
            lstReceipt.Items.Add("PARANAQUE CITY");
            lstReceipt.Items.Add("VAT Reg TIN 005-393-442-0000");
            lstReceipt.Items.Add("");
            lstReceipt.Items.Add("SOA # 00000000003456789012");
            lstReceipt.Items.Add("        STATEMENT OF ACCOUNT");
            lstReceipt.Items.Add("    For the Month of: " + d.MonthYear);
            lstReceipt.Items.Add("");
            lstReceipt.Items.Add("------- SERVICE INFORMATION -------");
            lstReceipt.Items.Add("Contract Account No : " + d.Contract);
            lstReceipt.Items.Add("Account Name        : " + d.Name);
            lstReceipt.Items.Add("Service Address     : " + d.Address);
            lstReceipt.Items.Add("TIN                 : " + d.TIN);
            lstReceipt.Items.Add("Rate Class          : " + d.RateClass);
            lstReceipt.Items.Add("Business Area       : " + d.Business);
            lstReceipt.Items.Add("");
            lstReceipt.Items.Add("------- METERING INFORMATION -------");
            lstReceipt.Items.Add("Meter No. : " + d.MeterNo);
            lstReceipt.Items.Add("MRU No.   : " + d.MRU);
            lstReceipt.Items.Add("Seq No.   : " + d.Seq);
            lstReceipt.Items.Add("Reading Date      : " + d.ReadingDate);
            lstReceipt.Items.Add("Present Reading   : " + d.Present);
            lstReceipt.Items.Add("Previous Reading  : " + d.Previous);
            lstReceipt.Items.Add("Consumption(cu.m) : " + d.Consumption);
            lstReceipt.Items.Add("");
            lstReceipt.Items.Add("------- BILL & PAYMENT HISTORY -------");
            lstReceipt.Items.Add("Desc   Total Amount   OR#   Date");
            lstReceipt.Items.Add("");
            lstReceipt.Items.Add("--------- BILLING SUMMARY ---------");
            lstReceipt.Items.Add("BILLING PERIOD: " + d.Period);
            lstReceipt.Items.Add(LR("Current Charges", d.Total));
            lstReceipt.Items.Add(LR("   Basic Charge", d.Basic));
            lstReceipt.Items.Add(LR("   Environmental Charges (20%)", d.Env));
            lstReceipt.Items.Add(LR("   Maintenance Service Charge", d.MSC));
            lstReceipt.Items.Add(LR("   Total Current Chgs Before Taxes", d.Before));
            lstReceipt.Items.Add(LR("   Government Taxes", d.Tax));
            lstReceipt.Items.Add("------------------------------------------");
            lstReceipt.Items.Add(LR("TOTAL AMOUNT DUE", "PHP " + d.Total));
            lstReceipt.Items.Add(LR("PAYMENT DUE DATE", d.DueDate));
        }

        // PRINT
        private void btnPrint_Click(object sender, EventArgs e)
        {
            ReceiptData d = GetData();
            if (d == null) return;
            Reciept f = new Reciept(d);
            f.ShowDialog();
        }

        // empty handlers left from the designer (keep them)
        private void textBox22_TextChanged(object sender, EventArgs e) { }
        private void label33_Click(object sender, EventArgs e) { }
        private void groupBox4_Enter(object sender, EventArgs e) { }
        private void label18_Click(object sender, EventArgs e) { }
        private void groupBox2_Enter(object sender, EventArgs e) { }
    }
}