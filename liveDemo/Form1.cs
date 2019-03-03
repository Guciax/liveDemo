using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static MST.MES.OrderStructureByOrderNo;

namespace liveDemo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.Columns.Add("a", "Zlecenie");
            dataGridView1.Columns.Add("s", "Kitting");
            dataGridView1.Columns.Add("d", "10NC");
            dataGridView1.Columns.Add("z", "Ilosc");
            dataGridView1.Columns.Add("x", "SMT");
            dataGridView1.Columns.Add("c", "Test");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var kittingData = MST.MES.SqlDataReaderMethods.Kitting.GetOrdersInfoByDataReader(7);
            var notFinishedOrder = kittingData.Where(o => o.Value.endDate.Year < 2017);
            var lgOrders = notFinishedOrder.Select(o=>o.Value).Where(o => o.productionPlanId != "0").GroupBy(o=>o.productionPlanId).ToDictionary(k=>k.Key, v=>v.ToList());
            var mstOrders = notFinishedOrder.Where(o => o.Value.productionPlanId == "0");

            var smtData = MST.MES.SqlDataReaderMethods.SMT.GetOrders(notFinishedOrder.Select(o => o.Key).ToArray());
            var testData = MST.MES.SqlDataReaderMethods.LedTest.GetTestRecordsForOrders(MST.MES.SqlDataReaderMethods.LedTest.TesterIdToName(),mstOrders.Select(o => o.Key).ToArray());

            var lgPlanFullInfo = MST.MES.SqlDataReaderMethods.Kitting.GetKittingDataForProductionPlan(lgOrders.Select(o => o.Key).ToArray());

            foreach (var order in mstOrders)
            {
                dataGridView1.Rows.Add(order.Value.orderNo, 
                                        order.Value.kittingDate, 
                                        order.Value.modelId_12NCFormat, 
                                        order.Value.orderedQty,
                                        smtData[order.Key].totalManufacturedQty,
                                        testData[order.Key].totalPcbsTestedCount);
            }

            foreach (var prodPlanEntry in lgOrders)
            {
                dataGridView1.Rows.Add(prodPlanEntry.Key, 
                    prodPlanEntry.Value.First().kittingDate, 
                    prodPlanEntry.Value.First().modelId_12NCFormat, 
                    lgPlanFullInfo[prodPlanEntry.Key].Select(o => o.orderedQty).Sum()
                    );
            }
        }

        
    }
}
