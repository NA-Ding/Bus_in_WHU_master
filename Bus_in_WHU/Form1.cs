using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Controls;

namespace Bus_in_WHU
{
    public partial class Form1 : Form
    {
        private int mMouseFlag; 

        public Form1()
        {
            InitializeComponent();
            axTOCControl1.SetBuddyControl(axMapControl1);
            axTOCControl1.EnableLayerDragDrop = true;
            SynchronizeEagleEye();

        }

        //属性查询
        private void button1_Click(object sender, EventArgs e)
        {
            IMap pMap = this.axMapControl1.Map;
            //定义图层,要素游标,查询过滤器,要素
            IFeatureLayer pFeatureLayer;
            ICompositeLayer pCompLayer;
            List<IFeatureLayer> pList = new List<IFeatureLayer>();

            IFeatureCursor pFeatureCursor;
            IQueryFilter pQueryFilter;
            IFeature pFeature;

            #region
            bool flag = false;

            Console.WriteLine(this.axMapControl1.LayerCount);
            //获取图层
            //清除上次查询结果
            this.axMapControl1.Map.ClearSelection();
            IActiveView pActiveView = this.axMapControl1.Map as IActiveView;

            for (int i = 0; i < this.axMapControl1.LayerCount; i++)
            {
                if (pMap.get_Layer(i) is IFeatureLayer)
                {
                    //获得图层要素
                    pFeatureLayer = pMap.get_Layer(i) as IFeatureLayer;
                    if (pFeatureLayer.Visible)
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
                            if (pFeatureLayer.Visible)
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
                                    if (pFeatureLayer.Visible)
                                        pList.Add(pFeatureLayer);
                                }
                            }
                        }
                    }
                }
            }
            Console.WriteLine(pList.Count);
            #endregion

            //实例化pQueryFilter并设置查询条件
            pQueryFilter = new QueryFilter();
            pQueryFilter.WhereClause = "name='"+this.textBox1.Text+"'";
            Console.WriteLine(pQueryFilter.WhereClause);

           //查询
            for (int i = 0; i < pList.Count; i++)
            {
                pFeatureLayer = pList[i];
                Console.WriteLine(pFeatureLayer.Name);
                pFeatureCursor = pFeatureLayer.Search(pQueryFilter, false);
                pFeature = pFeatureCursor.NextFeature();
                if (pFeature != null)
                {
                    axMapControl1.Map.SelectFeature(pFeatureLayer, pFeature);
                    axMapControl1.FlashShape(pFeature.Shape);
                    //this.axMapControl1.Extent = pFeature.Shape.Envelope;
                    flag = true;
                }
            }
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeoSelection, null, null);
            pActiveView.Refresh();//刷新图层
            if (!flag)
            {
                MessageBox.Show("没有找到" + this.textBox1.Text, "提示");
            }             
        }

        //空间查询
        private double ConvertPixeIToMapUnits(IActiveView activeView, double pixelUnits)
        {
            double realWorldDiaplayExtent;
            int pixelExtent;
            double sizeOfOnePixel;
            double mapUnits;
            ////获取设备中视图显示宽度,即像素个数
            pixelExtent = activeView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().right-activeView.ScreenDisplay.DisplayTransformation.get_DeviceFrame().left;
            //获取地图坐标系中地图显示范围
            realWorldDiaplayExtent =activeView.ScreenDisplay.DisplayTransformation.VisibleBounds.Width;
            //每个像素大小代表的实际距离
            sizeOfOnePixel = realWorldDiaplayExtent / pixelExtent;
            //地理距离
            mapUnits = pixelUnits * sizeOfOnePixel;
            return mapUnits;
        }

        private void 空间查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //标记点查询
            mMouseFlag = 1;
            //设置鼠标形状
            this.axMapControl1.MousePointer = ESRI.ArcGIS.Controls.esriControlsMousePointer.esriPointerCrosshair;
        }

        private void axMapControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            if (mMouseFlag == 1)
            {
                
         
                IMap pMap = this.axMapControl1.Map;
                //定义图层,要素游标,查询过滤器,要素
                ICompositeLayer pCompLayer;
                List<IFeatureLayer> pList = new List<IFeatureLayer>();
                Console.WriteLine(this.axMapControl1.LayerCount);
                //获取图层
                for (int i = 0; i < this.axMapControl1.LayerCount; i++)
                {
                    IFeatureLayer pFeatureLayer;
                    if (pMap.get_Layer(i) is IFeatureLayer)
                    {
                        //获得图层要素
                        pFeatureLayer = pMap.get_Layer(i) as IFeatureLayer;
                        if (pFeatureLayer.Visible)
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
                                if (pFeatureLayer.Visible)
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
                                        if (pFeatureLayer.Visible)
                                            pList.Add(pFeatureLayer);
                                    }
                                }
                            }
                        }
                    }
                }

                Console.WriteLine(pList.Count);

                for (int i = 0; i < pList.Count; i++)
                {
                    Console.WriteLine(pList[i].Name);
                    IFeatureLayer pFeatureLayer;
                    IFeatureClass pFeatureClass;
                    pFeatureLayer = pList[i] as IFeatureLayer;
                    pFeatureClass = pFeatureLayer.FeatureClass;
                    if (pFeatureClass == null)
                    {
                        return;
                    }
                    IActiveView pActiveView;
                    IPoint pPoint;
                    double length;

                    //获取视图范围
                    pActiveView = this.axMapControl1.ActiveView;
                    //获取鼠标点击屏幕坐标
                    pPoint = pActiveView.ScreenDisplay.DisplayTransformation.ToMapPoint(e.x, e.y);
                    //2个像素大小的屏幕距离转换为地图距离
                    length = ConvertPixeIToMapUnits(pActiveView, 2);
                    ITopologicalOperator pTopoOperator;
                    IGeometry pGeoBuffer;
                    ISpatialFilter pSpatialFilter;
                    //根据缓冲半径生成空间过滤器
                    pTopoOperator = pPoint as ITopologicalOperator;
                    pGeoBuffer = pTopoOperator.Buffer(length);
                    pSpatialFilter = new ESRI.ArcGIS.Geodatabase.SpatialFilterClass();
                    pSpatialFilter.Geometry = pGeoBuffer;
                    //根据图层类型训责缓冲方式
                    switch (pFeatureClass.ShapeType)
                    {
                        case esriGeometryType.esriGeometryPoint:
                            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                            break;
                        case esriGeometryType.esriGeometryPolyline:
                            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                            break;
                        case esriGeometryType.esriGeometryPolygon:
                            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            break;
                    }

                    //定义空间过滤器的空间字段
                    pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;
                    IQueryFilter pQueryFilter;
                    IFeatureCursor pFeatureCursor;
                    IFeature pFeature;
                    //利用要素过滤器查询要素
                    pQueryFilter = pSpatialFilter as IQueryFilter;
                    pFeatureCursor = pFeatureLayer.Search(pQueryFilter, true);
                    pFeature = pFeatureCursor.NextFeature();
                    int fieldIndex;
                    if (pFeature != null)
                    {
                        //选择指定要素
                        this.axMapControl1.Map.ClearSelection();
                        this.axMapControl1.Map.SelectFeature((ILayer)pFeatureLayer, pFeature);
                        this.axMapControl1.Refresh();
                        fieldIndex = pFeature.Fields.FindField("name");
                        MessageBox.Show("查找到 " +pFeatureLayer.Name+" 图层中的 "+ pFeature.get_Value(fieldIndex), "提示");
                    }
                }
               

            }
        }


        //鹰眼
        IEnvelope pEnvelope;
        private void SynchronizeEagleEye()
        {
            if (axMapControl2.LayerCount > 0)
            {
                axMapControl2.ClearLayers();
            }

            axMapControl2.SpatialReference = axMapControl1.SpatialReference;
            //保持鸟瞰图和数据视图图层顺序一致
            for (int i = axMapControl1.LayerCount - 1; i >= 0; i--)
            {
                axMapControl2.AddLayer(axMapControl1.get_Layer(i));
            }
            axMapControl2.Extent = axMapControl1.Extent;
            pEnvelope = axMapControl1.Extent;
            DrawRectangle(pEnvelope);
            axMapControl2.Refresh();
        }
        private void DrawRectangle(IEnvelope pEnvelope)
        {
            IGraphicsContainer pGraphicsContainer = axMapControl2.Map as IGraphicsContainer;
            IActiveView pActiveView = pGraphicsContainer as IActiveView;

            pGraphicsContainer.DeleteAllElements();
            IRectangleElement pRectangleElement = new RectangleElementClass();
            IElement pElement = pRectangleElement as IElement;
            pElement.Geometry = pEnvelope;

            IRgbColor pRgbColor = new RgbColorClass();
            pRgbColor.Red = 255;
            pRgbColor.Blue = 0;
            pRgbColor.Green = 0;
            pRgbColor.Transparency = 255;

            ILineSymbol pLineSymbol = new SimpleLineSymbolClass();
            pLineSymbol.Width = 3;
            pLineSymbol.Color = pRgbColor;

            pRgbColor.Red = 255;
            pRgbColor.Blue = 0;
            pRgbColor.Green = 0;
            pRgbColor.Transparency = 0;

            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Outline = pLineSymbol;
            pFillSymbol.Color = pRgbColor;

            IFillShapeElement pFillShapeElement = pElement as IFillShapeElement;
            pFillShapeElement.Symbol = pFillSymbol;

            pGraphicsContainer.AddElement((IElement)pFillShapeElement, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }
        private void axMapControl1_OnMapReplaced(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMapReplacedEvent e)
        {
            SynchronizeEagleEye();
        }
        private void axMapControl1_OnExtentUpdated(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            pEnvelope = (IEnvelope)e.newEnvelope;
            DrawRectangle(pEnvelope);
        }
        private void axMapControl2_OnMouseMove(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseMoveEvent e)
        {
            if (e.button == 1)
            {
                IPoint pPoint = new PointClass();
                pPoint.PutCoords(e.mapX, e.mapY);
                axMapControl1.CenterAt(pPoint);
                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
        }
        private void axMapControl2_OnMouseDown(object sender, ESRI.ArcGIS.Controls.IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 1)
            {
                IPoint pPoint = new PointClass();
                pPoint.PutCoords(e.mapX, e.mapY);
                axMapControl1.CenterAt(pPoint);
                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
            else if (e.button == 2)
            {
                IEnvelope pEnv = axMapControl2.TrackRectangle();
                axMapControl1.Extent = pEnv;
                axMapControl1.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            }
        }

        //属性表
        private ILayer pLayer;
        private void axTOCControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.ITOCControlEvents_OnMouseDownEvent e)
        {
            if (axMapControl1.LayerCount > 0)
            {
                esriTOCControlItem pItem = new esriTOCControlItem();
                pLayer = new FeatureLayerClass();
                IBasicMap pBasicMap = new MapClass();
                object pOther = new object();
                object pIndex = new object();
                // Returns the item in the TOCControl at the specified coordinates.
                axTOCControl1.HitTest(e.x, e.y, ref pItem, ref pBasicMap, ref pLayer, ref pOther, ref pIndex);
            }//TOCControl类的ITOCControl接口的HitTest方法
            if (e.button == 2)
            {
                contextMenuStrip1.Show(axTOCControl1, e.x, e.y);
            }
        }
        private void 打开属性表ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //传入图层，在右击事件里返回的图层
            FormAttr frm1 = new FormAttr(pLayer as IFeatureLayer);
            frm1.Show();
        }

        //线路查询
        private void 站点查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RouteQuery routeQuery = new RouteQuery(this.axMapControl1);
            routeQuery.Show();
        }




    }

    
}