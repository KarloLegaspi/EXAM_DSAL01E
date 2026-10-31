using System;
using System.Drawing;
using System.Windows.Forms;

namespace EXAM_DSAL01E
{
    // The receipt form (built entirely in code)
    public partial class Reciept : Form
    {
        Panel pnl;
        int y;

        public Reciept(ReceiptData d)
        {
            InitializeComponent();

            Text = "Statement of Account";
            ClientSize = new Size(470, 800);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.Gray;

            pnl = new Panel();
            pnl.SetBounds(10, 10, 450, 780);
            pnl.BackColor = Color.White;
            pnl.BorderStyle = BorderStyle.FixedSingle;
            pnl.AutoScroll = true;
            Controls.Add(pnl);

            Build(d);
        }

        private void Reciept_Load(object sender, EventArgs e) { }

        Label Lbl(string text, int x, int yy, int w, float size, bool bold,
                  ContentAlignment align, int h = 18)
        {
            Label l = new Label();
            l.Text = text;
            l.SetBounds(x, yy, w, h);
            l.Font = new Font("Arial", size, bold ? FontStyle.Bold : FontStyle.Regular);
            l.TextAlign = align;
            pnl.Controls.Add(l);
            return l;
        }

        void Line(int yy)
        {
            Label l = new Label();
            l.BorderStyle = BorderStyle.Fixed3D;
            l.SetBounds(10, yy, 410, 2);
            pnl.Controls.Add(l);
        }

        void Header(string text)
        {
            Lbl(text, 10, y, 410, 10, false, ContentAlignment.MiddleCenter);
            Line(y + 20);
            y += 28;
        }

        void Field(string caption, string value)
        {
            Lbl(caption, 15, y, 150, 9, false, ContentAlignment.MiddleLeft);
            Lbl(": " + value, 165, y, 255, 9, false, ContentAlignment.MiddleLeft);
            y += 20;
        }

        void Row(string left, string right, bool bold = false, float size = 9)
        {
            Lbl(left, 15, y, 290, size, bold, ContentAlignment.MiddleLeft);
            Lbl(right, 305, y, 115, size, bold, ContentAlignment.MiddleRight);
            y += 20;
        }

        void Build(ReceiptData d)
        {
            y = 8;

            Lbl("Advisory: Check payment WILL NOT BE ACCEPTED without the Statement of Account effective October 01, 2019",
                10, y, 410, 8, false, ContentAlignment.TopLeft, 30);
            y += 38;

            // Logo area (text only) + FIXED Maynilad address
            Lbl("MAYNILAD", 10, y, 200, 22, true, ContentAlignment.MiddleLeft, 40);
            Lbl("Maynilad Water Services, Inc.\n8398 DR A SANTOS AVE BF HOMES\nPARANAQUE CITY\nVAT Reg TIN 005-393-442-0000",
                215, y, 205, 7.5f, false, ContentAlignment.TopRight, 55);
            y += 62;

            Lbl("SOA # 00000000003456789012", 10, y, 410, 9, false, ContentAlignment.MiddleLeft);
            y += 24;
            Lbl("STATEMENT OF ACCOUNT", 10, y, 410, 12, true, ContentAlignment.MiddleCenter, 22);
            y += 22;
            Lbl("For the Month of: " + d.MonthYear, 10, y, 410, 11, false, ContentAlignment.MiddleCenter, 22);
            y += 34;

            // SERVICE INFORMATION
            Header("SERVICE INFORMATION");
            Lbl("Contract Account No :", 15, y, 150, 9, false, ContentAlignment.MiddleLeft, 22);
            Lbl(d.Contract, 170, y, 250, 12, true, ContentAlignment.MiddleLeft, 22);
            y += 24;
            Field("Account Name", d.Name);
            Field("Service Address", d.Address);
            Field("TIN", d.TIN);
            Field("Rate Class", d.RateClass);
            Field("Business Area", d.Business);
            y += 10;

            // METERING INFORMATION
            Header("METERING INFORMATION");
            Lbl("Meter No.", 15, y, 130, 8, false, ContentAlignment.MiddleLeft);
            Lbl("MRU No.", 170, y, 120, 8, false, ContentAlignment.MiddleLeft);
            Lbl("Seq No.", 310, y, 110, 8, false, ContentAlignment.MiddleLeft);
            y += 18;
            Lbl(d.MeterNo, 15, y, 150, 9, false, ContentAlignment.MiddleLeft);
            Lbl(d.MRU, 170, y, 130, 9, false, ContentAlignment.MiddleLeft);
            Lbl(d.Seq, 310, y, 110, 9, false, ContentAlignment.MiddleLeft);
            y += 24;
            Field("Reading Date", d.ReadingDate);
            Field("Present Reading", d.Present);
            Field("Previous Reading", d.Previous);
            Field("Consumption (cu.m)", d.Consumption);
            y += 6;
            Lbl("Previous 3 Months", 15, y, 300, 8, false, ContentAlignment.MiddleLeft, 16);
            y += 16;
            Lbl("Consumption", 15, y, 300, 8, false, ContentAlignment.MiddleLeft, 16);
            y += 26;

            // BILL & PAYMENT HISTORY (display only)
            Header("BILL & PAYMENT HISTORY");
            Lbl("Desc", 15, y, 80, 7, false, ContentAlignment.MiddleLeft);
            Lbl("Total Amount", 110, y, 100, 7, false, ContentAlignment.MiddleLeft);
            Lbl("OR#", 270, y, 60, 7, false, ContentAlignment.MiddleLeft);
            Lbl("Date", 360, y, 60, 7, false, ContentAlignment.MiddleLeft);
            y += 20;
            Lbl("Description: WB-Water Bill, SD-Guarantee Deposit, MISC-Reopening Fee, Connection Fee, Metering Charge",
                15, y, 405, 6.5f, false, ContentAlignment.TopLeft, 28);
            y += 36;

            // BILLING SUMMARY
            Header("BILLING SUMMARY");
            Lbl("BILLING PERIOD : " + d.Period, 15, y, 405, 9, false, ContentAlignment.MiddleLeft);
            y += 22;
            Row("Current Charges", d.Total, true);
            Row("      Basic Charge", d.Basic);
            Row("      Environmental Charges (20% of Basic Charge)", d.Env, false, 8);
            Row("      Maintenance Service Charge (MSC)", d.MSC, false, 8);
            Row("      Total Current Charges Before Taxes", d.Before, false, 8);
            Row("      Government Taxes", d.Tax);
            y += 20;

            // TOTAL (display stops at Payment Due Date)
            Line(y);
            y += 8;
            Lbl("TOTAL AMOUNT DUE", 15, y, 230, 11, true, ContentAlignment.MiddleLeft, 22);
            Lbl("PHP " + d.Total, 245, y, 175, 12, true, ContentAlignment.MiddleRight, 22);
            y += 26;
            Lbl("PAYMENT DUE DATE", 15, y, 230, 11, true, ContentAlignment.MiddleLeft, 22);
            Lbl(d.DueDate, 245, y, 175, 12, true, ContentAlignment.MiddleRight, 22);
        }
    }

    // Values passed from Form1 to the receipt
    public class ReceiptData
    {
        public string Contract, Name, TIN, Address, RateClass, Business;
        public string MeterNo, MRU, Seq, ReadingDate, Present, Previous, Consumption;
        public string MonthYear, Period;
        public string Basic, Env, MSC, Before, Tax, Total, DueDate;
    }
}