using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

namespace Bus_in_WHU
{
    public partial class RouteQuery : Form
    {
        public RouteQuery(AxMapControl mapControl)
        {
            InitializeComponent();
            this.mMapControl = mapControl;
        }

        private AxMapControl mMapControl;

        private void button1_Click(object sender, EventArgs e)
        {

            IMap pMap = mMapControl.Map;
            //定义图层,要素游标,查询过滤器,要素
            ICompositeLayer pCompLayer;
            List<IFeatureLayer> pList = new List<IFeatureLayer>();

            #region
            //获取图层
            for (int i = 0; i < mMapControl.LayerCount; i++)
            {
                IFeatureLayer pFeatureLayer;
                if (pMap.get_Layer(i) is IFeatureLayer)
                {
                    //获得图层要素
                    pFeatureLayer = pMap.get_Layer(i) as IFeatureLayer;
                    if (pFeatureLayer.Name.EndsWith("线路"))
                        pList.Add(pFeatureLayer);
                }
                else if (pMap.get_Layer(i) is IGroupLayer)
                {
                    //遍历图层组
                    pCompLayer = pMap.get_Layer(i) as ICompositeLayer;
                    for (int j = 0; j < pCompLayer.Count; j++)
                    {
                        if (pCompLayer.get_Layer(j) is IFeatureLayer)
                        {
                            pFeatureLayer = pCompLayer.get_Layer(j) as IFeatureLayer;
                            if (pFeatureLayer.Name.EndsWith("线路"))
                                pList.Add(pFeatureLayer);

                        }
                        else if (pCompLayer.get_Layer(j) is IGroupLayer)
                        {
                            ICompositeLayer pCompLayer2 = pCompLayer.get_Layer(j) as ICompositeLayer;
                            for (int k = 0; k < pCompLayer2.Count; k++)
                            {
                                if (pCompLayer2.get_Layer(k) is IFeatureLayer)
                                {
                                    pFeatureLayer = pCompLayer2.get_Layer(k) as IFeatureLayer;
                                    if (pFeatureLayer.Name.EndsWith("线路"))
                                        pList.Add(pFeatureLayer);
                                }
                            }
                        }
                    }
                }
            }
           

            Console.WriteLine("initial:"+pList.Count);
            for (int i = 0; i < pList.Count; i++)
            {
                Console.WriteLine(pList[i].Name);
            }
            #endregion

            IFeatureCursor pFeatureCursor1, pFeatureCursor2;
            IQueryFilter pQueryFilter1, pQueryFilter2;
            IFeature pFeature1, pFeature2;

            //实例化pQueryFilter并设置查询条件
            pQueryFilter1 = new QueryFilter();
            pQueryFilter1.WhereClause = "start='" + this.textBox1.Text + "'";
            Console.WriteLine(pQueryFilter1.WhereClause);
            //查询
            List<IFeatureLayer> pList1 = new List<IFeatureLayer>();
            for (int i = 0; i < pList.Count; i++)
            {
                IFeatureLayer pFeatureLayer = pList[i];
                Console.WriteLine(i+pFeatureLayer.Name);
                pFeatureCursor1 = pFeatureLayer.Search(pQueryFilter1, false);
                pFeature1 = pFeatureCursor1.NextFeature();
                if (pFeature1 != null)
                {
                    pList1.Add(pList[i]);
                }
            }
            Console.WriteLine("start："+pList1.Count);

            
            pQueryFilter2 = new QueryFilter();
            pQueryFilter2.WhereClause = "end='" + this.textBox2.Text + "'";
            Console.WriteLine(pQueryFilter2.WhereClause);
            //查询
            List<IFeatureLayer> pList2 = new List<IFeatureLayer>();
            for (int i = 0; i < pList1.Count; i++)
            {
                IFeatureLayer pFeatureLayer = pList1[i];
                Console.WriteLine(pFeatureLayer.Name);
                pFeatureCursor2 = pFeatureLayer.Search(pQueryFilter2, false);
                pFeature2 = pFeatureCursor2.NextFeature();
                if (pFeature2 != null)
                {
                    pList2.Add(pList1[i]);
                }
            }
            Console.WriteLine("end"+pList2.Count);

            string route = "";
            if (pList2.Count != 0)
            {
                for (int i = 0; i < pList2.Count; i++)
                {
                    route += pList2[i].Name + "\r\n";
                    Console.WriteLine(route);
                }
                textBox3.Text = route;
            }
            else
            {
                textBox3.Text = route;
                MessageBox.Show("未查找到从" + this.textBox1.Text + "到"+this.textBox2.Text+"的直达公交线路！","提示");
            }


            //选中要素
            mMapControl.Map.ClearSelection();
            IActiveView pActiveView = mMapControl.Map as IActiveView;
            for (int i = 0; i < pList2.Count; i++)
            {
                IFeatureCursor pFeatureCursor;
                IQueryFilter pQueryFilter;
                IFeature pFeature;

                pQueryFilter = new QueryFilter();
                pQueryFilter.WhereClause = "name='" + pList2[i].Name.Substring(0,3)+ "'";
                IFeatureLayer pFeatureLayer = pList2[i];
                Console.WriteLine(pList2[i].Name.Substring(0, 3));
                pFeatureCursor = pFeatureLayer.Search(pQueryFilter, false);
                pFeature = pFeatureCursor.NextFeature();      
                if (pFeature != null)
                {
                    mMapControl.Map.SelectFeature(pFeatureLayer, pFeature);
                }
            }
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            pActiveView.Refresh();//刷新图层

        }

       
        
    }
}
