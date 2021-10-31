using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using AyhanMak.Business.Abstract;
using AyhanMak.Business.Concrete;
using AyhanMak.Entities.Concrete;

namespace AyhanMak.WebFormsUI
{
    public partial class Main : MaterialForm
    {
        #region Değişkenler
        readonly MaterialSkinManager _materialSkinManager;

        private ILogServices _logServices;

        private IProductService _productService;
        private IInvoiceService _invoiceService;
        private IPriceService _priceService;
        private INewpriceService _newpriceService;

        private IBrandServices _brandServices;
        private IThicknessServices _thicknessServices;
        private IUnitServices _unitServices;

        private List<Product> _productServiceList;
        private List<Product> _productServiceListByBrand;

        private List<Invoice> _invoiceServiceList;
        private List<Invoice> _invoiceServiceListByProduct;
        private List<Invoice> _invoiceServiceListByReturn;
        private List<Invoice> _invoiceServiceListByMachine;
        private List<Invoice> _invoiceServiceListBySparePart;

        private List<Brand> _brandServicesList;
        private List<Thickness> _thicknessServicesList;
        private List<Unit> _unitServicesList;


        private List<CastProduct> _castProducts;
        private List<String> _castName;

        private Product _product;
        private Invoice _mInvoice;

        #endregion

        #region Main

        public Main()
        {
            InitializeComponent();

            _materialSkinManager = MaterialSkinManager.Instance;
            _materialSkinManager.EnforceBackcolorOnAllComponents = true;
            _materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            _materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo500, Primary.Indigo700, Primary.Indigo100,
                Accent.Pink200, TextShade.WHITE);
            _materialSkinManager.AddFormToManage(this);

            _logServices = new LogManager();

            _productService = new ProductManager();
            _invoiceService = new InvoiceManager();
            _priceService = new PriceManager();
            _newpriceService = new NewpriceManager();

            _brandServices = new BrandManager();
            _thicknessServices = new ThicknessManager();
            _unitServices = new UnitManager();


            getRefreshServices();
       

        }

        private void getRefreshServices()
        {
            _productServiceList = _productService.getAllByProductName();
            _productServiceListByBrand = _productService.GetProductByBrand();

            _invoiceServiceList = _invoiceService.GetAllById();
            _invoiceServiceListByProduct = _invoiceService.GetByCategory("URUN");
            _invoiceServiceListByReturn = _invoiceService.GetByCategory("IADE");
            _invoiceServiceListByMachine = _invoiceService.GetByCategory("MAKİNA");
            _invoiceServiceListBySparePart = _invoiceService.GetByCategory("YEDEK");

            _brandServicesList = _brandServices.GetAll();
            _thicknessServicesList = _thicknessServices.GetAll();
            _unitServicesList = _unitServices.GetAll();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            getStok();
        }

        private void mTController_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == tpStok.TabIndex)
            {
                loadProducts();
                tControlStok.SelectedIndex = 0;
            }
            else if (e.TabPageIndex == tpUrunSat.TabIndex)
            {
                getUrunSat();
            }
            else if (e.TabPageIndex == tpMakinaSat.TabIndex)
            {

            }
            else if (e.TabPageIndex == tpYedekParca.TabIndex)
            {

            }
            else if (e.TabPageIndex == tpIade.TabIndex)
            {
                getIade();
            }
            else if (e.TabPageIndex == tpFatura.TabIndex)
            {
                getFatura();
                tControlFatura.SelectedIndex = 0;

            }
            else if (e.TabPageIndex == tpAyarlar.TabIndex)
            {
                getAyarlar();
                tControlAyarlar.SelectedIndex = 0;

            }
        }
        #endregion

        #region Stok
        private void getStok()
        {
            loadProducts();
            reLoadDataGridViewName();
            loadStokEkle();
            getStokGetTabIndex();
            dgvStok.ClearSelection();
        }
        public void loadProducts()
        {
            dgvStok.DataSource = returnCastProducts(_castProducts, _productServiceListByBrand);
            remaininQuantity();
            dgvStok.ClearSelection();
        }
        private void getStokGetTabIndex()
        {
            this.tpStokEkle.TabIndex = 0;
            this.tpStokGuncelle.TabIndex = 1;
            this.tpStokSirala.TabIndex = 2;
        }
        private List<CastProduct> returnCastProducts(List<CastProduct> castProductName, List<Product> getProductNames)
        {
            castProductName = new List<CastProduct>();
            castProductName.Clear();
            foreach (Product product in getProductNames)
            {
                castProductName.Add(new CastProduct
                {
                    id = product.id,
                    colorcode = product.colorcode,
                    brandname4pro = product.brandname4pro,
                    thicknessname4pro = product.thicknessname4pro,
                    productname = product.productname,

                    maturityprice = convertTT(product.maturityprice.ToString("N4")),
                    cashprice = convertTT(product.cashprice.ToString("N4")),
                    stockquantity = convertTT(product.stockquantity.ToString("N4")),

                    unitname4pro = product.unitname4pro,
                    brandid4pro = product.brandid4pro,
                    thicknessid4pro = product.thicknessid4pro,
                    unitid4pro = product.unitid4pro
                });
            }

            return castProductName;
        }
        string convertTT(string b)
        {
            decimal d = Convert.ToDecimal(b);

            if (d.ToString().EndsWith("0000"))
            {
                return d.ToString("N0");
            }
            else if (d.ToString().EndsWith("000"))
            {
                return d.ToString("N1");
            }
            else if (d.ToString().EndsWith("00"))
            {
                return d.ToString("N2");
            }
            else if (d.ToString().EndsWith("0"))
            {
                return d.ToString("N3");
            }
            else
            {
                return d.ToString();
            }
        }
        private void remaininQuantity()
        {
            foreach (DataGridViewRow row in dgvStok.Rows)
            {
                if (row.Cells[4].Value.ToString() == "İPLİK")
                {
                    if (Convert.ToDecimal(row.Cells[7].Value) >= 12 && Convert.ToDecimal(row.Cells[7].Value) <= 24)
                    {
                        row.Cells[7].Style.BackColor = Color.Yellow;
                    }
                    else if (Convert.ToDecimal(row.Cells[7].Value) < 12)
                    {
                        row.Cells[7].Style.BackColor = Color.Red;
                    }
                }
            }
        }
        private void tControlStok_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == tpStokEkle.TabIndex)
            {
                loadStokEkle();
                loadProducts();
            }
            else if (e.TabPageIndex == tpStokGuncelle.TabIndex)
            {
                loadStokGuncelle();
            }
            else if (e.TabPageIndex == tpStokSirala.TabIndex)
            {
                loadStokSirala();
            }
        }

        private void getBrandServices(ComboBox comboBox)
        {
            comboBox.DataSource = _brandServicesList;
            comboBox.DisplayMember = "brandname";
            comboBox.ValueMember = "brandid";
            comboBox.SelectedValue = "";
        }

        private void getThicknessServices(ComboBox comboBox)
        {
            comboBox.DataSource = _thicknessServicesList;
            comboBox.DisplayMember = "thicknessname";
            comboBox.ValueMember = "thicknessid";
            comboBox.SelectedValue = "";
        }

        private void getUnitServices(ComboBox comboBox)
        {
            comboBox.DataSource = _unitServicesList;
            comboBox.DisplayMember = "unitname";
            comboBox.ValueMember = "unitid";
            comboBox.SelectedValue = "";
        }

        public void loadStokEkle()
        {
            getBrandServices(cbxStokMarkaEkle);

            getThicknessServices(cbxStokKalinlikEkle);

            getUnitServices(cbxStokBirimDegeriEkle);

            remaininQuantity();
            dgvStok.ClearSelection();

            tbxStokRenkKoduEkle.Clear();
            tbxStokUrunAdiEkle.Clear();
            tbxStokVadeEkle.Clear();
            tbxStokNakitEkle.Clear();
            tbxStokStokAdediEkle.Clear();
        }

        public void loadStokGuncelle()
        {
            getBrandServices(cbxStokMarkaGuncelle);

            getThicknessServices(cbxStokKalinlikGuncelle);

            getUnitServices(cbxStokBirimDegeriGuncelle);

            remaininQuantity();
            dgvStok.ClearSelection();

            lblStokRenkKoduGorunum.Text = "--------";
            tbxStokUrunAdiGuncelle.Clear();
            tbxStokVadeGuncelle.Clear();
            tbxStokNakitGuncelle.Clear();
            tbxStokStokEkleGuncelle.Clear();
            tbxStokStokCikarGuncelle.Clear();
        }
        public void loadStokSirala()
        {
            getBrandServices(cbxStokMarkaSirala);

            getThicknessServices(cbxStokKalinlikSirala);

            remaininQuantity();
            dgvStok.ClearSelection();

            tbxStokUrunAdiSirala.Clear();
            tbxStokRenkKoduSirala.Clear();
            tbxStokNakitSirala.Clear();
            tbxStokVadeSirala.Clear();
        }
        public void reLoadDataGridViewName()
        {
            this.dgvStok.Columns[0].HeaderText = "ID";
            this.dgvStok.Columns[1].HeaderText = "Renk Kodu";
            this.dgvStok.Columns[2].HeaderText = "Marka";
            this.dgvStok.Columns[3].HeaderText = "Kalınlık";
            this.dgvStok.Columns[4].HeaderText = "Ürün Adı";
            this.dgvStok.Columns[5].HeaderText = "Vade Fiyatı";
            this.dgvStok.Columns[6].HeaderText = "Nakit Fiyatı";
            this.dgvStok.Columns[7].HeaderText = "Stok Adedi";
            this.dgvStok.Columns[8].HeaderText = "Birim Değeri";
            this.dgvStok.Columns[9].HeaderText = "B";
            this.dgvStok.Columns[10].HeaderText = "T";
            this.dgvStok.Columns[11].HeaderText = "U";

            this.dgvStok.Columns[0].Visible = false;
            this.dgvStok.Columns[9].Visible = false;
            this.dgvStok.Columns[10].Visible = false;
            this.dgvStok.Columns[11].Visible = false;
        }
        private void tbxStokUrunAdiSirala_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tbxStokUrunAdiSirala.Text != "")
                {
                    dgvStok.DataSource = returnCastProducts(_castProducts, _productService.GetProductByProductName(tbxStokUrunAdiSirala.Text));
                    dgvStok.ClearSelection();
                    remaininQuantity();

                    _productServiceListByBrand = _productService.GetProductByBrand();
                }
                else
                {
                    loadProducts();
                }
            }
            catch
            {
                MessageBox.Show("Sıralama Başarısız!");
            }
        }
        private void tbxStokRenkKoduSirala_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tbxStokRenkKoduSirala.Text != "")
                {
                    dgvStok.DataSource = returnCastProducts(_castProducts, _productService.GetProductByAllProductId(tbxStokRenkKoduSirala.Text));
                    dgvStok.ClearSelection();
                    remaininQuantity();

                    _productServiceListByBrand = _productService.GetProductByBrand();
                }
                else
                {
                    loadProducts();
                }
            }
            catch
            {
                MessageBox.Show("Sıralama Başarısız!");
            }
        }
        private void cbxStokMarkaSirala_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbxStokMarkaSirala.Focused)
                {
                    if (cbxStokKalinlikSirala.SelectedValue != null && cbxStokMarkaSirala.SelectedValue != null)
                    {
                        dgvStok.DataSource = returnCastProducts(_castProducts, _productService.GetProductByBrandNameAndThickness(cbxStokMarkaSirala.Text, cbxStokKalinlikSirala.Text));
                        dgvStok.ClearSelection();
                        remaininQuantity();

                        _productServiceListByBrand = _productService.GetProductByBrand();
                    }
                    else if (cbxStokMarkaSirala.SelectedValue != null)
                    {
                        dgvStok.DataSource = returnCastProducts(_castProducts, _productService.GetProductByBrandName(cbxStokMarkaSirala.Text));
                        dgvStok.ClearSelection();
                        remaininQuantity();

                        _productServiceListByBrand = _productService.GetProductByBrand();
                    }
                    else
                    {
                        loadProducts();
                    }
                }
                    
            }
            catch
            {
                MessageBox.Show("Sıralama Başarısız!");
            }
        }
        private void cbxStokKalinlikSirala_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (cbxStokKalinlikSirala.Focused)
                {
                    if (cbxStokMarkaSirala.SelectedValue != null && cbxStokKalinlikSirala.SelectedValue != null)
                    {
                        dgvStok.DataSource = returnCastProducts(_castProducts, _productService.GetProductByBrandNameAndThickness(cbxStokMarkaSirala.Text, cbxStokKalinlikSirala.Text));
                        dgvStok.ClearSelection();
                        remaininQuantity();

                        _productServiceListByBrand = _productService.GetProductByBrand();
                    }
                    else if (cbxStokKalinlikSirala.SelectedValue != null)
                    {
                        dgvStok.DataSource = returnCastProducts(_castProducts, _productService.GetProductByAllProductThickness(cbxStokKalinlikSirala.Text));
                        dgvStok.ClearSelection();
                        remaininQuantity();

                        _productServiceListByBrand = _productService.GetProductByBrand();
                    }
                    else
                    {
                        loadProducts();
                    }
                }
            }
            catch
            {
                MessageBox.Show("Sıralama Başarısız!");
            }
        }
        private void dgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvStok.CurrentRow;
            if (row != null)
            {
                tbxStokUrunAdiGuncelle.Text = row.Cells[4].Value.ToString();
                cbxStokMarkaGuncelle.Text = row.Cells[2].Value.ToString();
                cbxStokKalinlikGuncelle.Text = row.Cells[3].Value.ToString();
                tbxStokVadeGuncelle.Text = row.Cells[5].Value.ToString();
                tbxStokNakitGuncelle.Text = row.Cells[6].Value.ToString();
                cbxStokBirimDegeriGuncelle.Text = row.Cells[8].Value.ToString();
                tbxStokStokCikarGuncelle.Text = "0";
                tbxStokStokEkleGuncelle.Text = "0";
                lblStokRenkKoduGorunum.Text = dgvStok.CurrentRow.Cells[1].Value.ToString();
            }
        }
        private void btnStokEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbxStokMarkaEkle.Text == "")
                {
                    MessageBox.Show("Marka Seçiniz.");
                }
                else if (tbxStokUrunAdiEkle.Text == "")
                {
                    MessageBox.Show("Ürün Adını Giriniz.");
                }
                else if (tbxStokRenkKoduEkle.Text == "")
                {
                    MessageBox.Show("Renk Kodunu Giriniz.");
                }
                else if (cbxStokKalinlikEkle.Text == "")
                {
                    MessageBox.Show("Kalınlık Seçiniz.");
                }
                else if (cbxStokBirimDegeriEkle.Text == "")
                {
                    MessageBox.Show("Birim Değeri Seçiniz.");
                }
                else if (tbxStokVadeEkle.Text == "")
                {
                    MessageBox.Show("Ürün Vade Fiyatını Giriniz.");
                }
                else if (tbxStokNakitEkle.Text == "")
                {
                    MessageBox.Show("Ürün Nakit Fiyatını Giriniz.");
                }
                else if (tbxStokStokAdediEkle.Text == "")
                {
                    MessageBox.Show("Stok Adedi Giriniz.");
                }
                else
                {
                    if (_productService.GetProductByColorCodeAndBrandAndThickness(tbxStokRenkKoduEkle.Text,
                        cbxStokMarkaEkle.Text, cbxStokKalinlikEkle.Text, tbxStokUrunAdiEkle.Text) == null)
                    {
                        btnStokEkle.Enabled = false;
                        _productService.Add(new Product
                        {
                            colorcode = tbxStokRenkKoduEkle.Text,
                            productname = tbxStokUrunAdiEkle.Text,
                            brandname4pro = cbxStokMarkaEkle.Text,
                            thicknessname4pro = cbxStokKalinlikEkle.Text,
                            cashprice = Convert.ToDecimal(tbxStokNakitEkle.Text.Replace(".", ",")),
                            maturityprice = Convert.ToDecimal(tbxStokVadeEkle.Text.Replace(".", ",")),
                            stockquantity = Convert.ToDecimal(tbxStokStokAdediEkle.Text.Replace(".", ",")),
                            unitname4pro = cbxStokBirimDegeriEkle.Text,
                            brandid4pro = _brandServices.Get(cbxStokMarkaEkle.Text).brandid,
                            thicknessid4pro = _thicknessServices.Get(cbxStokKalinlikEkle.Text).thicknessid,
                            unitid4pro = _unitServices.Get(cbxStokBirimDegeriEkle.Text).unitid
                        });


                        tbxStokRenkKoduEkle.Clear();
                        tbxStokUrunAdiEkle.Clear();
                        tbxStokVadeEkle.Clear();
                        tbxStokNakitEkle.Clear();
                        tbxStokStokAdediEkle.Clear();

                        _productServiceListByBrand = _productService.GetProductByBrand();
                        loadProducts();

                        MessageBox.Show("Ürün Eklendi.");
                        btnStokEkle.Enabled = true;
                    }
                    else
                    {
                        throw new Exception("Böyle bir ürün var");
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnStokGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbxStokMarkaGuncelle.Text == "")
                {
                    MessageBox.Show("Marka Seçiniz.");
                }
                else if (tbxStokUrunAdiGuncelle.Text == "")
                {
                    MessageBox.Show("Ürün Adını Giriniz.");
                }
                else if (cbxStokKalinlikGuncelle.Text == "")
                {
                    MessageBox.Show("Kalınlık Seçiniz.");
                }
                else if (cbxStokBirimDegeriGuncelle.Text == "")
                {
                    MessageBox.Show("Birim Değeri Seçiniz.");
                }
                else if (tbxStokVadeGuncelle.Text == "")
                {
                    MessageBox.Show("Ürün Vade Fiyatını Giriniz.");
                }
                else if (tbxStokNakitGuncelle.Text == "")
                {
                    MessageBox.Show("Ürün Nakit Fiyatını Giriniz.");
                }
                else if (tbxStokStokEkleGuncelle.Text == "")
                {
                    MessageBox.Show("Stok Ekleme Giriniz.");
                }
                else if (tbxStokStokCikarGuncelle.Text == "")
                {
                    MessageBox.Show("Stok Çıkarma Giriniz.");
                }
                else
                {
                    var row = dgvStok.CurrentRow;
                    if (row != null)
                    {
                        btnStokGuncelle.Enabled = false;
                        _productService.Update(new Product
                        {
                            id = Convert.ToInt32(row.Cells[0].Value),
                            colorcode = row.Cells[1].Value.ToString(),
                            brandname4pro = cbxStokMarkaGuncelle.Text,
                            thicknessname4pro = cbxStokKalinlikGuncelle.Text,
                            productname = tbxStokUrunAdiGuncelle.Text,
                            maturityprice = Convert.ToDecimal(tbxStokVadeGuncelle.Text.Replace(".", ",")),
                            cashprice = Convert.ToDecimal(tbxStokNakitGuncelle.Text.Replace(".", ",")),
                            stockquantity = Convert.ToDecimal(row.Cells[7].Value) + Convert.ToDecimal(tbxStokStokEkleGuncelle.Text.Replace(".", ",")) - Convert.ToDecimal(tbxStokStokCikarGuncelle.Text.Replace(".", ",")),
                            unitname4pro = cbxStokBirimDegeriGuncelle.Text,
                            brandid4pro = Convert.ToInt32(row.Cells[9].Value),
                            thicknessid4pro = Convert.ToInt32(row.Cells[10].Value),
                            unitid4pro = Convert.ToInt32(row.Cells[11].Value)
                        });

                        string logName = $"{row.Cells[1].Value.ToString()}, {cbxStokMarkaGuncelle.Text}, {cbxStokKalinlikGuncelle.Text}, {tbxStokUrunAdiGuncelle.Text}";
                        string logAdding = tbxStokStokEkleGuncelle.Text;
                        string logSelling = tbxStokStokCikarGuncelle.Text;
                        string logOldQuantity = row.Cells[7].Value.ToString();
                        string logNewQuantity = $"{Convert.ToDecimal(row.Cells[7].Value) + Convert.ToDecimal(tbxStokStokEkleGuncelle.Text.Replace(".", ",")) - Convert.ToDecimal(tbxStokStokCikarGuncelle.Text.Replace(".", ","))}";

                        _logServices.Add(new Log
                        {
                            logproductname = logName,
                            logadding = logAdding,
                            logselling = logSelling,
                            logoldquantity = logOldQuantity,
                            lognewquantity = logNewQuantity
                        });

                        tbxStokUrunAdiGuncelle.Clear();
                        tbxStokVadeGuncelle.Clear();
                        tbxStokNakitGuncelle.Clear();
                        tbxStokStokEkleGuncelle.Clear();
                        tbxStokStokCikarGuncelle.Clear();

                        _productServiceListByBrand = _productService.GetProductByBrand();
                        loadProducts();
                        MessageBox.Show("Ürün Güncellendi");
                        btnStokGuncelle.Enabled = true;
                    }
                }
            }
            catch
            {
                MessageBox.Show("Güncellerken Hata Oldu!");
            }
        }
        private void btnStokSil_Click(object sender, EventArgs e)
        {
            try
            {
                var row = dgvStok.CurrentRow;
                if (row != null)
                {
                    btnStokSil.Enabled = false;
                    _productService.Delete(new Product
                    {
                        id = Convert.ToInt32(row.Cells[0].Value),
                    });

                    tbxStokUrunAdiGuncelle.Clear();
                    tbxStokVadeGuncelle.Clear();
                    tbxStokNakitGuncelle.Clear();
                    tbxStokStokEkleGuncelle.Clear();

                    _productServiceListByBrand = _productService.GetProductByBrand();
                    loadProducts();
       
                    MessageBox.Show("Ürün Silindi");
                    btnStokSil.Enabled = true;
                }
            }
            catch
            {
                MessageBox.Show("Silerken Hata Oldu!");
            }
        }
        private void btnStokTopluGuncelle_Click(object sender, EventArgs e)
        {
            var selectedRows = dgvStok.SelectedRows
                .OfType<DataGridViewRow>()
                .Where(row => !row.IsNewRow)
                .ToArray();
            if (selectedRows.Length > 0)
            {
                if (tbxStokNakitSirala.Text != "" && tbxStokVadeSirala.Text != "")
                {
                    foreach (DataGridViewRow row in selectedRows)
                    {
                        btnStokTopluGuncelle.Enabled = false;
                        _productService.Update(new Product
                        {
                            id = Convert.ToInt32(row.Cells[0].Value),
                            colorcode = row.Cells[1].Value.ToString(),
                            brandname4pro = row.Cells[2].Value.ToString(),
                            thicknessname4pro = row.Cells[3].Value.ToString(),
                            productname = row.Cells[4].Value.ToString(),
                            maturityprice = Convert.ToDecimal(tbxStokVadeSirala.Text.Replace(".", ",")),
                            cashprice = Convert.ToDecimal(tbxStokNakitSirala.Text.Replace(".", ",")),
                            stockquantity = Convert.ToDecimal(row.Cells[7].Value),
                            unitname4pro = row.Cells[8].Value.ToString(),
                            brandid4pro = Convert.ToInt32(row.Cells[9].Value),
                            thicknessid4pro = Convert.ToInt32(row.Cells[10].Value),
                            unitid4pro = Convert.ToInt32(row.Cells[11].Value),
                        });

                    }
                    dgvStok.ClearSelection();
                    tbxStokNakitSirala.Clear();
                    tbxStokVadeSirala.Clear();

                    _productServiceListByBrand = _productService.GetProductByBrand();
                    loadProducts();

                    MessageBox.Show("Ürünler Güncellendi");
                    btnStokTopluGuncelle.Enabled = true;

                }
                else if (tbxStokNakitSirala.Text != "")
                {
                    foreach (DataGridViewRow row in selectedRows)
                    {
                        btnStokTopluGuncelle.Enabled = false;
                        _productService.Update(new Product
                        {
                            id = Convert.ToInt32(row.Cells[0].Value),
                            colorcode = row.Cells[1].Value.ToString(),
                            brandname4pro = row.Cells[2].Value.ToString(),
                            thicknessname4pro = row.Cells[3].Value.ToString(),
                            productname = row.Cells[4].Value.ToString(),
                            maturityprice = Convert.ToDecimal(row.Cells[5].Value),
                            cashprice = Convert.ToDecimal(tbxStokNakitSirala.Text.Replace(".", ",")),
                            stockquantity = Convert.ToDecimal(row.Cells[7].Value),
                            unitname4pro = row.Cells[8].Value.ToString(),
                            brandid4pro = Convert.ToInt32(row.Cells[9].Value),
                            thicknessid4pro = Convert.ToInt32(row.Cells[10].Value),
                            unitid4pro = Convert.ToInt32(row.Cells[11].Value),
                        });

                    }
                    dgvStok.ClearSelection();
                    tbxStokNakitSirala.Clear();
                    tbxStokVadeSirala.Clear();
                    loadProducts();

                    MessageBox.Show("Ürünler Güncellendi");
                    btnStokTopluGuncelle.Enabled = true;

                }
                else if (tbxStokVadeSirala.Text != "")
                {
                    foreach (DataGridViewRow row in selectedRows)
                    {
                        btnStokTopluGuncelle.Enabled = false;
                        _productService.Update(new Product
                        {
                            id = Convert.ToInt32(row.Cells[0].Value),
                            colorcode = row.Cells[1].Value.ToString(),
                            brandname4pro = row.Cells[2].Value.ToString(),
                            thicknessname4pro = row.Cells[3].Value.ToString(),
                            productname = row.Cells[4].Value.ToString(),
                            maturityprice = Convert.ToDecimal(tbxStokVadeSirala.Text.Replace(".", ",")),
                            cashprice = Convert.ToDecimal(row.Cells[6].Value),
                            stockquantity = Convert.ToDecimal(row.Cells[7].Value),
                            unitname4pro = row.Cells[8].Value.ToString(),
                            brandid4pro = Convert.ToInt32(row.Cells[9].Value),
                            thicknessid4pro = Convert.ToInt32(row.Cells[10].Value),
                            unitid4pro = Convert.ToInt32(row.Cells[11].Value),
                        });

                    }
                    dgvStok.ClearSelection();
                    tbxStokNakitSirala.Clear();
                    tbxStokVadeSirala.Clear();
                    loadProducts();

                    MessageBox.Show("Ürünler Güncellendi");
                    btnStokTopluGuncelle.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Lütfen Değiştirmek istediğiniz değeri giriniz");
                }

            }
        }

        #region StokTextMask
        private void tbxStokVadeEkle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void tbxStokNakitEkle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void tbxStokStokAdediEkle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void tbxStokVadeGuncelle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void tbxStokNakitGuncelle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void tbxStokStokEkleGuncelle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void tbxStokStokCikarGuncelle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void tbxStokVadeSirala_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void tbxStokNakitSirala_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void cbxStokMarkaSirala_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                cbxStokMarkaSirala.SelectedValue = "";
                _productServiceListByBrand = _productService.GetProductByBrand();
            }

        }
        private void cbxStokKalinlikSirala_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 8)
            {
                cbxStokKalinlikSirala.SelectedValue = "";
                _productServiceListByBrand = _productService.GetProductByBrand();
            }
        }
        #endregion

        #endregion

        #region UrunSat
        private void getUrunSat()
        {
            loadUrunSat();
        }
        public void loadUrunSat()
        {
            getBrandServices(cbxUrunSatMarka);

            getThicknessServices(cbxUrunSatKalinlik);

            cbxUrunSatUrunAdi.DataSource = returnCastString(_castName, _productServiceList);
        }
        private List<String> returnCastString(List<String> castProductName, List<Product> getProductNames)
        {
            castProductName = new List<String>();
            castProductName.Clear();
            foreach (Product product in getProductNames)
            {
                if (!castProductName.Contains(product.productname.ToUpper()))
                {
                    castProductName.Add(product.productname.ToUpper());
                }
                
            }

            return castProductName;
        }
        private void btnUrunSatEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxUrunSatRenkKodu.Text == "")
                {
                    MessageBox.Show("Lütfen Renk Kodu giriniz!");
                }
                else if (tbxUrunSatSatisAdedi.Text == "")
                {
                    MessageBox.Show("Lütfen Satılan Adedi giriniz!");
                }
                else
                {

                    string renkKodu = tbxUrunSatRenkKodu.Text;

                    if (_productService.GetProductByColorCodeAndBrandAndThickness(renkKodu, cbxUrunSatMarka.Text, cbxUrunSatKalinlik.Text, cbxUrunSatUrunAdi.Text) != null && dgvUrunSat.RowCount < 17)
                    {

                        _product = _productService.GetProductByColorCodeAndBrandAndThickness(renkKodu, cbxUrunSatMarka.Text, cbxUrunSatKalinlik.Text, cbxUrunSatUrunAdi.Text);

                        string urunAdi = _product.productname;
                        string kalinlik = _product.thicknessname4pro.ToString();
                        string marka = _product.brandname4pro.ToString();
                        decimal vadeli = _product.maturityprice;
                        decimal nakit = _product.cashprice;
                        decimal satisSayisi = Convert.ToDecimal(tbxUrunSatSatisAdedi.Text.Replace(".", ","));
                        string birimDegeri = _product.unitname4pro;
                        bool vade;
                        decimal fiyat;

                        if (rBtnUrunSatVade.Checked)
                        {
                            vade = true;
                            fiyat = vadeli;
                            dgvUrunSat.Rows.Add(urunAdi, renkKodu, kalinlik, marka, fiyat, satisSayisi, birimDegeri, vade);

                            tbxUrunSatRenkKodu.Clear();
                            tbxUrunSatSatisAdedi.Clear();
                        }
                        else if (rBtnUrunSatNakit.Checked)
                        {
                            vade = false;
                            fiyat = nakit;
                            dgvUrunSat.Rows.Add(urunAdi, renkKodu, kalinlik, marka, fiyat, satisSayisi, birimDegeri, vade);

                            tbxUrunSatRenkKodu.Clear();
                            tbxUrunSatSatisAdedi.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Lütfen Vadeliyi, Ya da Nakit'i Seçiniz");
                        }

                    }
                    else
                    {
                        tbxUrunSatRenkKodu.Clear();
                        tbxUrunSatSatisAdedi.Clear();

                        throw new Exception("Böyle Bir Ürün Yok! yada fazla ürün girdiniz!");
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnUrunSatSil_Click(object sender, EventArgs e)
        {
            if (dgvUrunSat.CurrentRow != null)
            {
                try
                {
                    dgvUrunSat.Rows.RemoveAt(dgvUrunSat.CurrentRow.Index);
                    MessageBox.Show("Ürün Silindi!");
                }
                catch
                {
                    MessageBox.Show("Ürün Silinemedi");
                }
            }
        }
        private void btnUrunSatFaturaKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxUrunSatFirma.Text != "")
                {
                    if (dgvUrunSat.Rows != null)
                    {
                        btnUrunSatFaturaKaydet.Enabled = false;
                        decimal toplamfiyat = 0;
                        for (int i = 0; i < dgvUrunSat.Rows.Count; i++)
                        {
                            toplamfiyat += Convert.ToDecimal(dgvUrunSat.Rows[i].Cells[4].Value) * Convert.ToDecimal(dgvUrunSat.Rows[i].Cells[5].Value);
                        }

                        lblUrunSatToplamFiyat.Text = $"Toplam Fiyat : {convertTT(toplamfiyat.ToString("N4"))} TL";

                        _mInvoice = new Invoice
                        {
                            date = DateTime.Now,
                            totalprice = toplamfiyat,
                            customer = tbxUrunSatFirma.Text,
                            category = "URUN"
                        };

                        _invoiceService.Add(_mInvoice);

                        for (int i = 0; i < dgvUrunSat.Rows.Count; i++)
                        {
                            _product = _productService.GetProductByColorCodeAndBrandAndThickness(dgvUrunSat.Rows[i].Cells[1].Value.ToString(), dgvUrunSat.Rows[i].Cells[3].Value.ToString(), dgvUrunSat.Rows[i].Cells[2].Value.ToString(), dgvUrunSat.Rows[i].Cells[0].Value.ToString());
                            _priceService.Add(new Price
                            {
                                pinovoiceid = _mInvoice.inovoiceid,
                                colorcode = _product.colorcode,
                                productname = _product.productname,
                                productgenus = _product.thicknessname4pro.ToString(),
                                productbrand = _product.brandname4pro.ToString(),
                                productprice = Convert.ToDecimal(dgvUrunSat.Rows[i].Cells[4].Value),
                                salesquantity = Convert.ToDecimal(dgvUrunSat.Rows[i].Cells[5].Value),
                                unitid = _product.unitname4pro,
                                maturity = Convert.ToBoolean(dgvUrunSat.Rows[i].Cells[7].Value),

                            });

                            _productService.Update(new Product
                            {
                                id = _product.id,
                                colorcode = _product.colorcode,
                                productname = _product.productname,
                                thicknessname4pro = _product.thicknessname4pro,
                                brandname4pro = _product.brandname4pro,
                                maturityprice = _product.maturityprice,
                                cashprice = _product.cashprice,
                                stockquantity = _product.stockquantity - Convert.ToDecimal(dgvUrunSat.Rows[i].Cells[5].Value),
                                unitname4pro = _product.unitname4pro,
                                brandid4pro = _product.brandid4pro,
                                thicknessid4pro = _product.thicknessid4pro,
                                unitid4pro = _product.unitid4pro,
                            });

                            string logName = $"{_product.colorcode}, {_product.brandname4pro}, {_product.thicknessname4pro}, {_product.productname}";
                            string logAdding = "0";
                            string logSelling = $"{dgvUrunSat.Rows[i].Cells[5].Value.ToString()}";
                            string logOldQuantity = _product.stockquantity.ToString();
                            string logNewQuantity = $"{_product.stockquantity - Convert.ToDecimal(dgvUrunSat.Rows[i].Cells[5].Value)}";

                            _logServices.Add(new Log
                            {
                                logproductname = logName,
                                logadding = logAdding,
                                logselling = logSelling,
                                logoldquantity = logOldQuantity,
                                lognewquantity = logNewQuantity
                            });
                        }

                        DialogResult result = MessageBox.Show("Fiş yazdırmak ister misiniz?", "Print Plug",
                            MessageBoxButtons.YesNo);
                        
                        if (result == DialogResult.Yes)
                        {
                            var ps = new PageSettings();
                            var papersizes = new PaperSize("A6", 815, 565);
                            ps.PaperSize = papersizes;
                            ps.Margins.Top = 0;
                            ps.Margins.Bottom = 0;
                            ps.Margins.Left = 5;
                            ps.Margins.Right = 5;
                            documentUrunSat.DefaultPageSettings = ps;
                            documentUrunSat.Print();
                        }

                        rBtnUrunSatVade.Checked = false;
                        rBtnUrunSatNakit.Checked = false;

                        dgvUrunSat.Rows.Clear();
                        tbxUrunSatFirma.Clear();

                        _invoiceServiceList = _invoiceService.GetAllById();
                        _invoiceServiceListByProduct = _invoiceService.GetByCategory("URUN");

                        _productServiceListByBrand = _productService.GetProductByBrand();

                        lblUrunSatToplamFiyat.Text = $"Toplam Fiyat :";

                        btnUrunSatFaturaKaydet.Enabled = true;

                    }
                }
                else
                {
                    throw new Exception("Lütfen müşteriyi giriniz?");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void documentUrunSat_PrintPage(object sender, PrintPageEventArgs e)
        {
            int invoiceIdInRow = _mInvoice.inovoiceid;
            urunSatPage(e, invoiceIdInRow);
        }
        private void urunSatPage(PrintPageEventArgs e, int invoiceIdInRow)
        {

            string path = Application.StartupPath.ToString() + @"\images\logo.png";
            Image newImage = Image.FromFile(path);
            Point ulCorner = new Point(20, 20);
            e.Graphics.DrawImage(newImage, ulCorner);

            Font font = new Font("Arial", 9, FontStyle.Bold);
            SolidBrush firca = new SolidBrush(Color.Black);
            Pen kalem = new Pen(Color.Black);
            e.Graphics.DrawString("(0224) 715 75 95", font, firca, 30, 110);
            e.Graphics.DrawString("TESLİM FİŞİ", font, firca, 650, 10);

            e.Graphics.DrawLine(kalem, 400, 40, 800, 40);
            e.Graphics.DrawString("FİŞ NO", font, firca, 400, 50);
            e.Graphics.DrawString($"=      {_invoiceService.getByInvoiceId(invoiceIdInRow).inovoiceid}", font, firca, 520, 50);

            e.Graphics.DrawLine(kalem, 400, 70, 800, 70);
            e.Graphics.DrawString("MÜŞTERİ", font, firca, 400, 80);
            e.Graphics.DrawString($"=      {_invoiceService.getByInvoiceId(invoiceIdInRow).customer}", font, firca, 520, 80);

            e.Graphics.DrawLine(kalem, 400, 100, 800, 100);
            e.Graphics.DrawString("FİŞ TARİHİ", font, firca, 400, 110);
            e.Graphics.DrawString($"=      {_invoiceService.getByInvoiceId(invoiceIdInRow).date}", font, firca, 520, 110);

            e.Graphics.DrawLine(kalem, 400, 130, 800, 130);

            int yExen = 0;
            int xExen = 0;
            List<Price> allPricesByInvoice = _priceService.GetByInvoiceId(invoiceIdInRow);

            int xExenMarka = 0;
            int xExenKalinlik = 0;
            int xExenRenkKodu = 0;
            int xExenSatisAdedi = 0;
            int xExenBirimFiyati = 0;
            int xExenToplamFiyat = 0;
            foreach (Price price in allPricesByInvoice)
            {
                if (price.productname.Length >= 10)
                {
                    xExenMarka = 80;
                    xExenKalinlik = 90;
                    xExenRenkKodu = 30;
                    xExenSatisAdedi = 40;
                    xExenBirimFiyati = 40;
                    if (price.productbrand.Length >= 10)
                    {
                        xExenKalinlik = 150;
                        xExenRenkKodu = 90;
                        xExenSatisAdedi = 70;
                        xExenBirimFiyati = 60;
                        xExenToplamFiyat = 20;
                    }
                }
                else if (price.productbrand.Length >= 10)
                {
                    xExenKalinlik = 80;
                    xExenRenkKodu = 40;
                    xExenSatisAdedi = 20;
                    xExenBirimFiyati = 30;
                }
            }

            foreach (Price price in allPricesByInvoice)
            {
                e.Graphics.DrawString($"{price.productname}", font, firca, 20 + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{price.productbrand}", font, firca, 20 + xExenMarka + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{price.productgenus}", font, firca, 20 + xExenKalinlik + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{price.colorcode}", font, firca, 20 + xExenRenkKodu + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{convertTT(price.salesquantity.ToString("N4"))} {price.unitid}", font, firca, 20 + xExenSatisAdedi + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{convertTT(price.productprice.ToString("N4"))} TL", font, firca, 20 + xExenBirimFiyati + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{convertTT((price.salesquantity * price.productprice).ToString("N4"))} TL", font, firca, 20 + xExenToplamFiyat + xExen, 190 + yExen);
                xExen = 0;
                yExen += 20;
            }

            e.Graphics.DrawString("Ürün İsmi", font, firca, 20, 150);
            e.Graphics.DrawString("Marka", font, firca, 135 + xExenMarka, 150);
            e.Graphics.DrawString("Kalınlık", font, firca, 250 + xExenKalinlik, 150);
            e.Graphics.DrawString("Renk Kodu", font, firca, 365 + xExenRenkKodu, 150);
            e.Graphics.DrawString("Satış Adedi", font, firca, 480 + xExenSatisAdedi, 150);
            e.Graphics.DrawString("Birim Fiyatı", font, firca, 595 + xExenBirimFiyati, 150);
            e.Graphics.DrawString("Toplam Fiyat", font, firca, 710 + xExenToplamFiyat, 150);
            e.Graphics.DrawLine(kalem, 20, 180, 820, 180);

            e.Graphics.DrawLine(kalem, 20, 190 + yExen, 820, 190 + yExen);
            e.Graphics.DrawString($"Toplam Fiyat = {convertTT(_invoiceService.getByInvoiceId(invoiceIdInRow).totalprice.ToString("N4"))} TL", font, firca, 560, 200 + yExen);
        }

        #region UrunSatTextMask
        private void tbxUrunSatSatisAdedi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        #endregion

        #endregion

        #region MakinaSat
        private void btnMakinaSatEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxMakinaSatMarka.Text == "")
                {
                    MessageBox.Show("Lütfen Marka giriniz!");
                }
                else if (tbxMakinaSatAciklama.Text == "")
                {
                    MessageBox.Show("Lütfen Açıklama giriniz!");
                }
                else if (tbxMakinaSatAdet.Text == "")
                {
                    MessageBox.Show("Lütfen Adet giriniz!");
                }
                else if (tbxMakinaSatFiyat.Text == "")
                {
                    MessageBox.Show("Lütfen Fiyat giriniz!");
                }
                else
                {

                    string marka = tbxMakinaSatMarka.Text;
                    string aciklama = tbxMakinaSatAciklama.Text;
                    string adet = tbxMakinaSatAdet.Text.Replace(".", ",");
                    string birimFiyat = tbxMakinaSatFiyat.Text.Replace(".", ",");
                    string toplamFiyat = (Convert.ToDecimal(adet) * Convert.ToDecimal(birimFiyat)).ToString();


                    if (rBtnMakinaSatMakinaAlim.Checked)
                    {
                        dgvMakinaSatAlinanMakinalar.Rows.Add(marka, aciklama, adet, birimFiyat, toplamFiyat);

                        tbxMakinaSatMarka.Clear();
                        tbxMakinaSatAdet.Clear();
                        tbxMakinaSatAciklama.Clear();
                        tbxMakinaSatFiyat.Clear();
                    }
                    else if (rBtnMakinaSatMakinaSatim.Checked)
                    {
                        dgvMakinaSatSatilanMakinalar.Rows.Add(marka, aciklama, adet, birimFiyat, toplamFiyat);

                        tbxMakinaSatMarka.Clear();
                        tbxMakinaSatAdet.Clear();
                        tbxMakinaSatAciklama.Clear();
                        tbxMakinaSatFiyat.Clear();

                    }
                    else
                    {
                        MessageBox.Show("Lütfen Ürün Alım Satım bilgisini seçin");
                    }


                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnMakinaSatSil_Click(object sender, EventArgs e)
        {
            if (dgvMakinaSatSatilanMakinalar.CurrentRow != null)
            {
                if (dgvMakinaSatSatilanMakinalar.CurrentRow.Selected)
                {
                    try
                    {
                        dgvMakinaSatSatilanMakinalar.Rows.RemoveAt(dgvMakinaSatSatilanMakinalar.CurrentRow.Index);
                        MessageBox.Show("Ürün Silindi!");
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }

            if (dgvMakinaSatAlinanMakinalar.CurrentRow != null)
            {
                if (dgvMakinaSatAlinanMakinalar.CurrentRow.Selected)
                {
                    try
                    {
                        dgvMakinaSatAlinanMakinalar.Rows.RemoveAt(dgvMakinaSatAlinanMakinalar.CurrentRow.Index);
                        MessageBox.Show("Ürün Silindi!");
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }
        }
        private void dgvMakinaSatSatilanMakinalar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvMakinaSatSatilanMakinalar.CurrentRow;
            if (row.Selected)
            {
                dgvMakinaSatAlinanMakinalar.ClearSelection();
            }
        }
        private void dgvMakinaSatAlinanMakinalar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvMakinaSatAlinanMakinalar.CurrentRow;
            if (row.Selected)
            {
                dgvMakinaSatSatilanMakinalar.ClearSelection();
            }
        }
        private void btnMakinaSatFaturaKaydet_Click(object sender, EventArgs e)
        {
            if (tbxMakinaSatFirma.Text == "")
            {
                MessageBox.Show("Lütfen Firma Giriniz");
            }
            else if (tbxMakinaSatIletisim.Text == "")
            {
                MessageBox.Show("Lütfen İletişim Bilgilerini Giriniz");
            }
            else
            {
                var rowSel = dgvMakinaSatSatilanMakinalar.CurrentRow;
                var rowBuy = dgvMakinaSatAlinanMakinalar.CurrentRow;
                if (rowSel != null && rowBuy != null)
                {
                    btnMakinaSatFaturaKaydet.Enabled = false;
                    _mInvoice = new Invoice
                    {
                        date = DateTime.Now,
                        totalprice = getTotalMakinaSat(),
                        customer = tbxMakinaSatFirma.Text,
                        category = "MAKİNA"
                    };
                    _invoiceService.Add(_mInvoice);

                    sellMakinaSat(_mInvoice);

                    buyMakinaSat(_mInvoice);
                    
                    getMakinaSatPrinter();

                    _invoiceServiceList = _invoiceService.GetAllById();
                    _invoiceServiceListByMachine = _invoiceService.GetByCategory("MAKİNA");

                    btnMakinaSatFaturaKaydet.Enabled = true;
                }
                else if (rowSel != null)
                {
                    btnMakinaSatFaturaKaydet.Enabled = false;
                    _mInvoice = new Invoice
                    {
                        date = DateTime.Now,
                        totalprice = getTotalMakinaSat(),
                        customer = tbxMakinaSatFirma.Text,
                        category = "MAKİNA"
                    };
                    _invoiceService.Add(_mInvoice);

                    sellMakinaSat(_mInvoice);
                    
                    getMakinaSatPrinter();

                    _invoiceServiceList = _invoiceService.GetAllById();
                    _invoiceServiceListByMachine = _invoiceService.GetByCategory("MAKİNA");

                    btnMakinaSatFaturaKaydet.Enabled = true;
                }
                else if (rowBuy != null)
                {
                    btnMakinaSatFaturaKaydet.Enabled = false;
                    _mInvoice = new Invoice
                    {
                        date = DateTime.Now,
                        totalprice = getTotalMakinaSat(),
                        customer = tbxMakinaSatFirma.Text,
                        category = "MAKİNA"
                    };
                    _invoiceService.Add(_mInvoice);

                    buyMakinaSat(_mInvoice);
                   
                    getMakinaSatPrinter();

                    _invoiceServiceList = _invoiceService.GetAllById();
                    _invoiceServiceListByMachine = _invoiceService.GetByCategory("MAKİNA");

                    btnMakinaSatFaturaKaydet.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Lütfen Bir Ürün Giriniz");
                }
            }
        }
        private Decimal getTotalMakinaSat()
        {
            Decimal toplamSatis = 0;
            for (int i = 0; i < dgvMakinaSatSatilanMakinalar.Rows.Count; i++)
            {
                toplamSatis += (Convert.ToDecimal(dgvMakinaSatSatilanMakinalar.Rows[i].Cells[2].Value) * Convert.ToDecimal(dgvMakinaSatSatilanMakinalar.Rows[i].Cells[3].Value));
            }

            Decimal toplamAlis = 0;
            for (int i = 0; i < dgvMakinaSatAlinanMakinalar.Rows.Count; i++)
            {
                toplamAlis += (Convert.ToDecimal(dgvMakinaSatAlinanMakinalar.Rows[i].Cells[2].Value) * Convert.ToDecimal(dgvMakinaSatAlinanMakinalar.Rows[i].Cells[3].Value));
            }

            return toplamSatis - toplamAlis;
        }
        private void sellMakinaSat(Invoice mInvoice)
        {
            for (int i = 0; i < dgvMakinaSatSatilanMakinalar.Rows.Count; i++)
            {
                _newpriceService.Add(new Newprice
                {
                    npinovoiceid = mInvoice.inovoiceid,
                    npbrand = dgvMakinaSatSatilanMakinalar.Rows[i].Cells[0].Value.ToString(),
                    npcomment = dgvMakinaSatSatilanMakinalar.Rows[i].Cells[1].Value.ToString(),
                    npquantity = Convert.ToDecimal(dgvMakinaSatSatilanMakinalar.Rows[i].Cells[2].Value),
                    npcash = Convert.ToDecimal(dgvMakinaSatSatilanMakinalar.Rows[i].Cells[3].Value),
                    nptotalprice = (Convert.ToDecimal(dgvMakinaSatSatilanMakinalar.Rows[i].Cells[2].Value) * Convert.ToDecimal(dgvMakinaSatSatilanMakinalar.Rows[i].Cells[3].Value)),
                    npcompany = tbxMakinaSatFirma.Text,
                    npcontact = tbxMakinaSatIletisim.Text,
                    nptype = true
                });
            }
        }
        private void buyMakinaSat(Invoice mInvoice)
        {
            for (int i = 0; i < dgvMakinaSatAlinanMakinalar.Rows.Count; i++)
            {
                _newpriceService.Add(new Newprice
                {
                    npinovoiceid = mInvoice.inovoiceid,
                    npbrand = dgvMakinaSatAlinanMakinalar.Rows[i].Cells[0].Value.ToString(),
                    npcomment = dgvMakinaSatAlinanMakinalar.Rows[i].Cells[1].Value.ToString(),
                    npquantity = Convert.ToDecimal(dgvMakinaSatAlinanMakinalar.Rows[i].Cells[2].Value),
                    npcash = Convert.ToDecimal(dgvMakinaSatAlinanMakinalar.Rows[i].Cells[3].Value),
                    nptotalprice = (Convert.ToDecimal(dgvMakinaSatAlinanMakinalar.Rows[i].Cells[2].Value) * Convert.ToDecimal(dgvMakinaSatAlinanMakinalar.Rows[i].Cells[3].Value)),
                    npcompany = tbxMakinaSatFirma.Text,
                    npcontact = tbxMakinaSatIletisim.Text,
                    nptype = false
                });
            }
        }
        private void getMakinaSatPrinter()
        {
            var ps = new PageSettings();
            var papersizes = new PaperSize("A6", 815, 565);
            ps.PaperSize = papersizes;
            ps.Margins.Top = 0;
            ps.Margins.Bottom = 0;
            ps.Margins.Left = 5;
            ps.Margins.Right = 5;
            documentMakinaSat.DefaultPageSettings = ps;
            documentMakinaSat.Print();

            dgvMakinaSatAlinanMakinalar.Rows.Clear();
            dgvMakinaSatSatilanMakinalar.Rows.Clear();
            tbxMakinaSatFirma.Clear();
            tbxMakinaSatIletisim.Clear();
            rBtnMakinaSatMakinaSatim.Checked = false;
            rBtnMakinaSatMakinaAlim.Checked = false;
        }
        private void documentMakinaSat_PrintPage(object sender, PrintPageEventArgs e)
        {
            makinaSatPage(e, _mInvoice.inovoiceid);
        }
        private void makinaSatPage(PrintPageEventArgs e, int mInvoiceId)
        {
            Invoice mInvoice = _invoiceService.getByInvoiceId(mInvoiceId);
            string path = Application.StartupPath.ToString() + @"\images\logo.png";
            Image newImage = Image.FromFile(path);
            Point ulCorner = new Point(20, 20);
            e.Graphics.DrawImage(newImage, ulCorner);

            Font font = new Font("Arial", 9, FontStyle.Bold);
            SolidBrush firca = new SolidBrush(Color.Black);
            Pen kalem = new Pen(Color.Black);
            e.Graphics.DrawString("(0224) 715 75 95", font, firca, 30, 110);
            e.Graphics.DrawString("TESLİM FİŞİ", font, firca, 650, 10);

            e.Graphics.DrawLine(kalem, 400, 40, 800, 40);
            e.Graphics.DrawString("Firma", font, firca, 400, 50);
            e.Graphics.DrawString($"=      {mInvoice.customer}", font, firca, 520, 50);

            e.Graphics.DrawLine(kalem, 400, 70, 800, 70);
            e.Graphics.DrawString("Firma İletişim", font, firca, 400, 80);
            e.Graphics.DrawString($"=      {_newpriceService.GetByInvoiceId(mInvoiceId)[0].npcontact}", font, firca, 520, 80);

            e.Graphics.DrawLine(kalem, 400, 100, 800, 100);
            e.Graphics.DrawString("Fiş Tarihi", font, firca, 400, 110);
            e.Graphics.DrawString($"=      {DateTime.Now}", font, firca, 520, 110);

            e.Graphics.DrawLine(kalem, 400, 130, 800, 130);

            int yExen = 0;
            int xExen = 0;

            int xExenAciklama = 0;
            int xExenAdet = 0;
            int xExenBirimFiyati = 0;
            int xExenToplamFiyat = 0;
            var satList = _newpriceService.GetByInvoiceIdAndSellType(mInvoiceId, true);
            var alList = _newpriceService.GetByInvoiceIdAndSellType(mInvoiceId, false);
            if (satList.Count != 0)
            {
                foreach (Newprice newprice in satList)
                {
                    if (newprice.npbrand.Length >= 18)
                    {
                        xExenAciklama = 90;
                        xExenAdet = 110;
                        xExenBirimFiyati = 50;
                        xExenToplamFiyat = 30;
                        if (newprice.npcomment.Length >= 18)
                        {
                            xExenAdet = 190;
                            xExenBirimFiyati = 110;
                            xExenToplamFiyat = 60;
                        }
                    }
                    else if (newprice.npcomment.Length >= 18)
                    {
                        xExenAciklama = 20;
                        xExenAdet = 100;
                        xExenBirimFiyati = 60;
                        xExenToplamFiyat = 30;
                    }
                }
            }
            else if (alList.Count != 0)
            {
                foreach (Newprice newprice in alList)
                {
                    if (newprice.npbrand.Length >= 18)
                    {
                        xExenAciklama = 90;
                        xExenAdet = 110;
                        xExenBirimFiyati = 50;
                        xExenToplamFiyat = 30;
                        if (newprice.npcomment.Length >= 18)
                        {
                            xExenAdet = 190;
                            xExenBirimFiyati = 110;
                            xExenToplamFiyat = 60;
                        }
                    }
                    else if (newprice.npcomment.Length >= 18)
                    {
                        xExenAciklama = 20;
                        xExenAdet = 100;
                        xExenBirimFiyati = 60;
                        xExenToplamFiyat = 30;
                    }
                }
            }


            foreach (Newprice newprice in satList)
            {
                e.Graphics.DrawString($"{newprice.npbrand}", font, firca, 20 + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{newprice.npcomment}", font, firca, 20 + xExenAciklama + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{convertTT(newprice.npquantity.ToString("N4"))}", font, firca, 20 + xExenAdet + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{convertTT(newprice.npcash.ToString("N4"))} TL", font, firca, 20 + xExenBirimFiyati + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{convertTT((newprice.npquantity * newprice.npcash).ToString("N4"))} TL", font, firca, 20 + xExenToplamFiyat + xExen, 190 + yExen);

                xExen = 0;
                yExen += 20;
            }

            if (alList.Count != 0)
            {
                if (satList.Count != 0)
                {
                    e.Graphics.DrawString("Satış Listesi", font, firca, 240, 120);
                    e.Graphics.DrawLine(kalem, 230, 135, 320, 135);

                    e.Graphics.DrawLine(kalem, 20, 190 + yExen, 820, 190 + yExen);
                    yExen += 50;
                    e.Graphics.DrawString("Alış Listesi", font, firca, 240, 160 + yExen);
                    e.Graphics.DrawLine(kalem, 230, 175 + yExen, 320, 175 + yExen);
                    e.Graphics.DrawLine(kalem, 20, 190 + yExen, 820, 190 + yExen);
                    yExen -= 50;
                    yExen += 60;
                }
                else
                {
                    e.Graphics.DrawString("Alış Listesi", font, firca, 240, 120);
                    e.Graphics.DrawLine(kalem, 230, 135, 320, 135);
                }

            }
            else
            {
                e.Graphics.DrawString("Satış Listesi", font, firca, 240, 120);
                e.Graphics.DrawLine(kalem, 230, 135, 320, 135);
            }

            foreach (Newprice newprice in alList)
            {
                e.Graphics.DrawString($"{newprice.npbrand}", font, firca, 20 + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{newprice.npcomment}", font, firca, 20 + xExenAciklama + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{convertTT(newprice.npquantity.ToString("N4"))}", font, firca, 20 + xExenAdet + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{convertTT(newprice.npcash.ToString("N4"))} TL", font, firca, 20 + xExenBirimFiyati + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{convertTT((newprice.npquantity * newprice.npcash).ToString("N4"))} TL", font, firca, 20 + xExenToplamFiyat + xExen, 190 + yExen);

                xExen = 0;
                yExen += 20;
            }

            e.Graphics.DrawString("Marka", font, firca, 20, 150);
            e.Graphics.DrawString("Açıklama", font, firca, 180 + xExenAciklama, 150);
            e.Graphics.DrawString("Adet", font, firca, 340 + xExenAdet, 150);
            e.Graphics.DrawString("Birim Fiyatı", font, firca, 500 + xExenBirimFiyati, 150);
            e.Graphics.DrawString("Toplam Fiyat", font, firca, 660 + xExenToplamFiyat, 150);
            e.Graphics.DrawLine(kalem, 20, 180, 820, 180);

            e.Graphics.DrawLine(kalem, 20, 190 + yExen, 820, 190 + yExen);
            e.Graphics.DrawString($"Toplam Fiyat = {convertTT(mInvoice.totalprice.ToString("N4"))} TL", font, firca, 560, 200 + yExen);
        }

        #region MakinaSatTextMask

        private void tbxMakinaSatAdet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }
        private void tbxMakinaSatFiyat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        #endregion

        #endregion

        #region YedekSat
        private void btnYedekEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxYedekMarka.Text == "")
                {
                    MessageBox.Show("Lütfen Marka giriniz!");
                }
                else if (tbxYedekAciklama.Text == "")
                {
                    MessageBox.Show("Lütfen Açıklama giriniz!");
                }
                else if (tbxYedekAdet.Text == "")
                {
                    MessageBox.Show("Lütfen Adet giriniz!");
                }
                else if (tbxYedekFiyat.Text == "")
                {
                    MessageBox.Show("Lütfen Fiyat giriniz!");
                }
                else
                {

                    string marka = tbxYedekMarka.Text;
                    string aciklama = tbxYedekAciklama.Text;
                    string adet = tbxYedekAdet.Text.Replace(".", ",");
                    string birimFiyat = tbxYedekFiyat.Text.Replace(".", ",");
                    string toplamFiyat = (Convert.ToDecimal(adet) * Convert.ToDecimal(birimFiyat)).ToString();

                    dgvYedek.Rows.Add(marka, aciklama, adet, birimFiyat, toplamFiyat);


                    tbxYedekMarka.Clear();
                    tbxYedekAdet.Clear();
                    tbxYedekAciklama.Clear();
                    tbxYedekFiyat.Clear();
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnYedekSil_Click(object sender, EventArgs e)
        {
            if (dgvYedek.CurrentRow != null)
            {
                if (dgvYedek.CurrentRow.Selected)
                {
                    try
                    {
                        dgvYedek.Rows.RemoveAt(dgvYedek.CurrentRow.Index);
                        MessageBox.Show("Ürün Silindi!");
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            }
        }
        private void btnYedekFaturaKaydet_Click(object sender, EventArgs e)
        {
            if (tbxYedekFirma.Text == "")
            {
                MessageBox.Show("Lütfen Firma Giriniz");
            }
            else
            {
                var row = dgvYedek.Rows;
                if (row.Count != 0)
                {
                    btnYedekFaturaKaydet.Enabled = false;
                    Decimal toplamSatis = 0;
                    for (int i = 0; i < row.Count; i++)
                    {
                        toplamSatis += Convert.ToDecimal(row[i].Cells[4].Value);
                    }
                    _mInvoice = new Invoice
                    {
                        date = DateTime.Now,
                        totalprice = toplamSatis,
                        customer = tbxYedekFirma.Text,
                        category = "YEDEK"
                    };
                    _invoiceService.Add(_mInvoice);
                    for (int i = 0; i < row.Count; i++)
                    {
                        _newpriceService.Add(new Newprice
                        {
                            npinovoiceid = _mInvoice.inovoiceid,
                            npbrand = row[i].Cells[0].Value.ToString(),
                            npcomment = row[i].Cells[1].Value.ToString(),
                            npquantity = Convert.ToDecimal(row[i].Cells[2].Value),
                            npcash = Convert.ToDecimal(row[i].Cells[3].Value),
                            nptotalprice = (Convert.ToDecimal(row[i].Cells[2].Value) * Convert.ToDecimal(row[i].Cells[3].Value)),
                            npcompany = tbxYedekFirma.Text,
                            npcontact = "(Yedek Parça Satış)",
                            nptype = true
                        });
                    }

                    _invoiceServiceList = _invoiceService.GetAllById();
                    _invoiceServiceListBySparePart = _invoiceService.GetByCategory("YEDEK");

                    getYedekPrinter();

                    btnYedekFaturaKaydet.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Lütfen Bir Ürün Giriniz");
                }

            }
        }
        private void getYedekPrinter()
        {
            var ps = new PageSettings();
            var papersizes = new PaperSize("A6", 815, 565);
            ps.PaperSize = papersizes;
            ps.Margins.Top = 0;
            ps.Margins.Bottom = 0;
            ps.Margins.Left = 5;
            ps.Margins.Right = 5;
            documentYedekSat.DefaultPageSettings = ps;
            documentYedekSat.Print();

            tbxYedekFirma.Clear();
            dgvYedek.Rows.Clear();
        }
        private void documentYedekSat_PrintPage(object sender, PrintPageEventArgs e)
        {
            yedekSatPage(e, _mInvoice.inovoiceid);
        }
        private void yedekSatPage(PrintPageEventArgs e, int mInvoiceId)
        {
            Invoice mInvoice = _invoiceService.getByInvoiceId(mInvoiceId);
            string path = Application.StartupPath.ToString() + @"\images\logo.png";
            Image newImage = Image.FromFile(path);
            Point ulCorner = new Point(20, 20);
            e.Graphics.DrawImage(newImage, ulCorner);

            Font font = new Font("Arial", 9, FontStyle.Bold);
            SolidBrush firca = new SolidBrush(Color.Black);
            Pen kalem = new Pen(Color.Black);
            e.Graphics.DrawString("(0224) 715 75 95", font, firca, 30, 110);
            e.Graphics.DrawString("TESLİM FİŞİ", font, firca, 650, 10);

            e.Graphics.DrawLine(kalem, 400, 40, 800, 40);
            e.Graphics.DrawString("Firma", font, firca, 400, 50);
            e.Graphics.DrawString($"=      {mInvoice.customer}", font, firca, 520, 50);

            e.Graphics.DrawLine(kalem, 400, 70, 800, 70);
            e.Graphics.DrawString("Firma İletişim", font, firca, 400, 80);
            e.Graphics.DrawString($"=      YOK", font, firca, 520, 80);

            e.Graphics.DrawLine(kalem, 400, 100, 800, 100);
            e.Graphics.DrawString("Fiş Tarihi", font, firca, 400, 110);
            e.Graphics.DrawString($"=      {DateTime.Now}", font, firca, 520, 110);

            e.Graphics.DrawLine(kalem, 400, 130, 800, 130);

            int yExen = 0;
            int xExen = 0;

            int xExenAciklama = 0;
            int xExenAdet = 0;
            int xExenBirimFiyati = 0;
            int xExenToplamFiyat = 0;
            var satList = _newpriceService.GetByInvoiceId(mInvoiceId);
            foreach (Newprice newprice in satList)
            {
                if (newprice.npbrand.Length >= 18)
                {
                    xExenAciklama = 90;
                    xExenAdet = 110;
                    xExenBirimFiyati = 50;
                    xExenToplamFiyat = 30;
                    if (newprice.npcomment.Length >= 18)
                    {
                        xExenAdet = 190;
                        xExenBirimFiyati = 110;
                        xExenToplamFiyat = 60;
                    }
                }
                else if (newprice.npcomment.Length >= 18)
                {
                    xExenAciklama = 20;
                    xExenAdet = 100;
                    xExenBirimFiyati = 60;
                    xExenToplamFiyat = 30;
                }
            }

            foreach (Newprice newprice in satList)
            {
                e.Graphics.DrawString($"{newprice.npbrand}", font, firca, 20 + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{newprice.npcomment}", font, firca, 20 + xExenAciklama + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{convertTT(newprice.npquantity.ToString("N4"))}", font, firca, 20 + xExenAdet + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{convertTT(newprice.npcash.ToString("N4"))} TL", font, firca, 20 + xExenBirimFiyati + xExen, 190 + yExen);
                xExen += 160;

                e.Graphics.DrawString($"{convertTT((newprice.npquantity * newprice.npcash).ToString("N4"))} TL", font, firca, 20 + xExenToplamFiyat + xExen, 190 + yExen);

                xExen = 0;
                yExen += 20;
            }

            e.Graphics.DrawString("Marka", font, firca, 20, 150);
            e.Graphics.DrawString("Açıklama", font, firca, 180 + xExenAciklama, 150);
            e.Graphics.DrawString("Adet", font, firca, 340 + xExenAdet, 150);
            e.Graphics.DrawString("Birim Fiyatı", font, firca, 500 + xExenBirimFiyati, 150);
            e.Graphics.DrawString("Toplam Fiyat", font, firca, 660 + xExenToplamFiyat, 150);
            e.Graphics.DrawLine(kalem, 20, 180, 820, 180);

            e.Graphics.DrawLine(kalem, 20, 190 + yExen, 820, 190 + yExen);
            e.Graphics.DrawString($"Toplam Fiyat = {convertTT(mInvoice.totalprice.ToString("N4"))} TL", font, firca, 560, 200 + yExen);
        }

        #region YedekSatTextMask
        private void tbxYedekAdet_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void tbxYedekFiyat_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        #endregion

        #endregion

        #region Iade
        private void getIade()
        {
            loadIade();
        }
        public void loadIade()
        {
            getBrandServices(cbxIadeMarka);

            getThicknessServices(cbxIadeKalinlik);
            
            cbxIadeUrunAdi.DataSource = returnCastString(_castName, _productServiceList);
        }
        private void btnIadeEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxIadeRenkKodu.Text == "")
                {
                    MessageBox.Show("Lütfen Renk Kodu giriniz!");
                }
                else if (tbxIadeIadeAdedi.Text == "")
                {
                    MessageBox.Show("Lütfen Satılan Adedi giriniz!");
                }
                else
                {

                    string renkKodu = tbxIadeRenkKodu.Text;

                    if (_productService.GetProductByColorCodeAndBrandAndThickness(renkKodu, cbxIadeMarka.Text, cbxIadeKalinlik.Text, cbxIadeUrunAdi.Text) != null && dgvIade.RowCount < 17)
                    {

                        _product = _productService.GetProductByColorCodeAndBrandAndThickness(renkKodu, cbxIadeMarka.Text, cbxIadeKalinlik.Text, cbxIadeUrunAdi.Text);

                        string urunAdi = _product.productname;
                        string kalinlik = _product.thicknessname4pro.ToString();
                        string marka = _product.brandname4pro.ToString();
                        string vadeli = _product.maturityprice.ToString();
                        string nakit = _product.cashprice.ToString();
                        string satisSayisi = tbxIadeIadeAdedi.Text.Replace(".", ",");
                        string birimDegeri = _product.unitname4pro;
                        bool vade;
                        string fiyat;

                        if (rBtnIadeVade.Checked)
                        {
                            vade = true;
                            fiyat = vadeli;
                            dgvIade.Rows.Add(urunAdi, renkKodu, kalinlik, marka, fiyat, satisSayisi, birimDegeri, vade);

                            tbxIadeRenkKodu.Clear();
                            tbxIadeIadeAdedi.Clear();
                        }
                        else if (rBtnIadeNakit.Checked)
                        {
                            vade = false;
                            fiyat = nakit;
                            dgvIade.Rows.Add(urunAdi, renkKodu, kalinlik, marka, fiyat, satisSayisi, birimDegeri, vade);

                            tbxIadeRenkKodu.Clear();
                            tbxIadeIadeAdedi.Clear();
                        }
                        else
                        {
                            MessageBox.Show("Lütfen Vadeli ya da Nakit seçenegini seçin");
                        }
                    }
                    else
                    {
                        tbxIadeRenkKodu.Clear();
                        tbxIadeIadeAdedi.Clear();
                        throw new Exception("Böyle Bir Ürün Yok! yada fazla ürün girdiniz!");
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnIadeSil_Click(object sender, EventArgs e)
        {
            if (dgvIade.CurrentRow != null)
            {
                try
                {
                    dgvIade.Rows.RemoveAt(dgvIade.CurrentRow.Index);
                    MessageBox.Show("Ürün Silindi!");
                }
                catch
                {
                    MessageBox.Show("Ürün Silinemedi");
                }
            }
        }
        private void btnIadeFaturaKaydet_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxIadeFirma.Text != "")
                {
                    if (dgvIade.Rows != null)
                    {
                        btnIadeFaturaKaydet.Enabled = false;
                        decimal toplamfiyat = 0;
                        for (int i = 0; i < dgvIade.Rows.Count; i++)
                        {
                            toplamfiyat += Convert.ToDecimal(dgvIade.Rows[i].Cells[4].Value) * Convert.ToDecimal(dgvIade.Rows[i].Cells[5].Value);
                        }
                        _mInvoice = new Invoice
                        {
                            date = DateTime.Now,
                            totalprice = toplamfiyat,
                            customer = tbxIadeFirma.Text,
                            category = "IADE"
                        };

                        _invoiceService.Add(_mInvoice);

                        for (int i = 0; i < dgvIade.Rows.Count; i++)
                        {
                            _product = _productService.GetProductByColorCodeAndBrandAndThickness(dgvIade.Rows[i].Cells[1].Value.ToString(), dgvIade.Rows[i].Cells[3].Value.ToString(), dgvIade.Rows[i].Cells[2].Value.ToString(), dgvIade.Rows[i].Cells[0].Value.ToString());
                            _priceService.Add(new Price
                            {
                                pinovoiceid = _mInvoice.inovoiceid,
                                colorcode = _product.colorcode,
                                productname = _product.productname,
                                productgenus = _product.thicknessname4pro.ToString(),
                                productbrand = _product.brandname4pro.ToString(),
                                productprice = Convert.ToDecimal(dgvIade.Rows[i].Cells[4].Value),
                                salesquantity = Convert.ToDecimal(dgvIade.Rows[i].Cells[5].Value),
                                unitid = _product.unitname4pro,
                                maturity = Convert.ToBoolean(dgvIade.Rows[i].Cells[7].Value),

                            });

                            _productService.Update(new Product
                            {
                                id = _product.id,
                                colorcode = _product.colorcode,
                                productname = _product.productname,
                                thicknessname4pro = _product.thicknessname4pro,
                                brandname4pro = _product.brandname4pro,
                                maturityprice = _product.maturityprice,
                                cashprice = _product.cashprice,
                                stockquantity = _product.stockquantity + Convert.ToDecimal(dgvIade.Rows[i].Cells[5].Value),
                                unitname4pro = _product.unitname4pro,
                                brandid4pro = _product.brandid4pro,
                                thicknessid4pro = _product.thicknessid4pro,
                                unitid4pro = _product.unitid4pro,
                            });

                            string logName = $"{_product.colorcode}, {_product.brandname4pro}, {_product.thicknessname4pro}, {_product.productname}";
                            string logAdding = $"{dgvIade.Rows[i].Cells[5].Value.ToString()}";
                            string logSelling = "0";
                            string logOldQuantity = _product.stockquantity.ToString();
                            string logNewQuantity = $"{_product.stockquantity + Convert.ToDecimal(dgvIade.Rows[i].Cells[5].Value)}";

                            _logServices.Add(new Log
                            {
                                logproductname = logName,
                                logadding = logAdding,
                                logselling = logSelling,
                                logoldquantity = logOldQuantity,
                                lognewquantity = logNewQuantity
                            });
                        }

                        DialogResult result = MessageBox.Show("Fiş yazdırmak ister misiniz?", "Print Plug",
                            MessageBoxButtons.YesNo);
                        
                        if (result == DialogResult.Yes)
                        {
                            var ps = new PageSettings();
                            var papersizes = new PaperSize("A6", 815, 565);
                            ps.PaperSize = papersizes;
                            ps.Margins.Top = 0;
                            ps.Margins.Bottom = 0;
                            ps.Margins.Left = 5;
                            ps.Margins.Right = 5;
                            documentIade.DefaultPageSettings = ps;
                            documentIade.Print();
                        }

                        _invoiceServiceList = _invoiceService.GetAllById();
                        _invoiceServiceListByReturn = _invoiceService.GetByCategory("IADE");

                        _productServiceListByBrand = _productService.GetProductByBrand();

                        dgvIade.Rows.Clear();
                        tbxIadeFirma.Clear();
                        rBtnIadeNakit.Checked = false;
                        rBtnIadeVade.Checked = false;

                        btnIadeFaturaKaydet.Enabled = true;
                    }
                }
                else
                {
                    throw new Exception("Lütfen müşteriyi giriniz?");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void documentIade_PrintPage(object sender, PrintPageEventArgs e)
        {
            int invoiceIdInRow = _mInvoice.inovoiceid;
            iadePage(e, invoiceIdInRow);
        }
        private void iadePage(PrintPageEventArgs e, int invoiceIdInRow)
        {

            string path = Application.StartupPath.ToString() + @"\images\logo.png";
            Image newImage = Image.FromFile(path);
            Point ulCorner = new Point(20, 20);
            e.Graphics.DrawImage(newImage, ulCorner);

            Font font = new Font("Arial", 9, FontStyle.Bold);
            SolidBrush firca = new SolidBrush(Color.Black);
            Pen kalem = new Pen(Color.Black);
            e.Graphics.DrawString("(0224) 715 75 95", font, firca, 30, 110);
            e.Graphics.DrawString("TESLİM FİŞİ", font, firca, 650, 10);

            firca = new SolidBrush(Color.Black);
            font = new Font("Arial", 20, FontStyle.Bold);
            e.Graphics.DrawString("ÜRÜN İADESİ", font, firca, 170, 70);
            font = new Font("Arial", 9, FontStyle.Bold);
            firca = new SolidBrush(Color.Black);

            e.Graphics.DrawLine(kalem, 400, 40, 800, 40);
            e.Graphics.DrawString("FİŞ NO", font, firca, 400, 50);
            e.Graphics.DrawString($"=      {_invoiceService.getByInvoiceId(invoiceIdInRow).inovoiceid}", font, firca, 520, 50);

            e.Graphics.DrawLine(kalem, 400, 70, 800, 70);
            e.Graphics.DrawString("MÜŞTERİ", font, firca, 400, 80);
            e.Graphics.DrawString($"=      {_invoiceService.getByInvoiceId(invoiceIdInRow).customer}", font, firca, 520, 80);

            e.Graphics.DrawLine(kalem, 400, 100, 800, 100);
            e.Graphics.DrawString("FİŞ TARİHİ", font, firca, 400, 110);
            e.Graphics.DrawString($"=      {_invoiceService.getByInvoiceId(invoiceIdInRow).date}", font, firca, 520, 110);

            e.Graphics.DrawLine(kalem, 400, 130, 800, 130);

            int yExen = 0;
            int xExen = 0;
            List<Price> allPricesByInvoice = _priceService.GetByInvoiceId(invoiceIdInRow);

            int xExenMarka = 0;
            int xExenKalinlik = 0;
            int xExenRenkKodu = 0;
            int xExenSatisAdedi = 0;
            int xExenBirimFiyati = 0;
            int xExenToplamFiyat = 0;
            foreach (Price price in allPricesByInvoice)
            {
                if (price.productname.Length >= 10)
                {
                    xExenMarka = 80;
                    xExenKalinlik = 90;
                    xExenRenkKodu = 30;
                    xExenSatisAdedi = 40;
                    xExenBirimFiyati = 40;
                    if (price.productbrand.Length >= 10)
                    {
                        xExenKalinlik = 150;
                        xExenRenkKodu = 90;
                        xExenSatisAdedi = 70;
                        xExenBirimFiyati = 60;
                        xExenToplamFiyat = 20;
                    }
                }
                else if (price.productbrand.Length >= 10)
                {
                    xExenKalinlik = 80;
                    xExenRenkKodu = 40;
                    xExenSatisAdedi = 20;
                    xExenBirimFiyati = 30;
                }
            }

            foreach (Price price in allPricesByInvoice)
            {
                e.Graphics.DrawString($"{price.productname}", font, firca, 20 + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{price.productbrand}", font, firca, 20 + xExenMarka + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{price.productgenus}", font, firca, 20 + xExenKalinlik + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{price.colorcode}", font, firca, 20 + xExenRenkKodu + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{convertTT(price.salesquantity.ToString("N4"))} {price.unitid}", font, firca, 20 + xExenSatisAdedi + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{convertTT(price.productprice.ToString("N4"))} TL", font, firca, 20 + xExenBirimFiyati + xExen, 190 + yExen);
                xExen += 115;

                e.Graphics.DrawString($"{convertTT((price.salesquantity * price.productprice).ToString("N4"))} TL", font, firca, 20 + xExenToplamFiyat + xExen, 190 + yExen);
                xExen = 0;
                yExen += 20;
            }

            e.Graphics.DrawString("Ürün İsmi", font, firca, 20, 150);
            e.Graphics.DrawString("Marka", font, firca, 135 + xExenMarka, 150);
            e.Graphics.DrawString("Kalınlık", font, firca, 250 + xExenKalinlik, 150);
            e.Graphics.DrawString("Renk Kodu", font, firca, 365 + xExenRenkKodu, 150);
            e.Graphics.DrawString("Satış Adedi", font, firca, 480 + xExenSatisAdedi, 150);
            e.Graphics.DrawString("Birim Fiyatı", font, firca, 595 + xExenBirimFiyati, 150);
            e.Graphics.DrawString("Toplam Fiyat", font, firca, 710 + xExenToplamFiyat, 150);
            e.Graphics.DrawLine(kalem, 20, 180, 820, 180);

            e.Graphics.DrawLine(kalem, 20, 190 + yExen, 820, 190 + yExen);
            e.Graphics.DrawString($"Toplam Fiyat = {convertTT(_invoiceService.getByInvoiceId(invoiceIdInRow).totalprice.ToString("N4"))} TL", font, firca, 560, 200 + yExen);
        }

        #region IadeTextMask
        private void tbxIadeSatisAdedi_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && e.KeyChar != 8 && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        #endregion

        #endregion

        #region Fatura

        private void getFatura()
        {
            loadFatura();
            getFaturaGetTabIndex();
            reLoadFaturaTumFaturaTableRows();
        }
        private void loadFatura()
        {
            dgvFaturaTumFatura.DataSource = _invoiceServiceList;
        }
        private void loadFaturaUrun()
        {
            dgvFaturaUrunFatura.DataSource = _invoiceServiceListByProduct;
        }
        private void loadFaturaIade()
        {
            dgvFaturaIadeFatura.DataSource = _invoiceServiceListByReturn;
        }
        private void loadFaturaMakine()
        {
            dgvFaturaMakinaFatura.DataSource = _invoiceServiceListByMachine;
        }
        private void loadFaturaYedek()
        {
            dgvFaturaYedekFatura.DataSource = _invoiceServiceListBySparePart;
        }
        public void reLoadFaturaTumFaturaTableRows()
        {
            this.dgvFaturaTumFatura.Columns[0].HeaderText = "Fatura ID";
            this.dgvFaturaTumFatura.Columns[1].HeaderText = "Fatura Tarihi";
            this.dgvFaturaTumFatura.Columns[2].HeaderText = "Toplam Fiyat";
            this.dgvFaturaTumFatura.Columns[3].HeaderText = "Müşteri";
            this.dgvFaturaTumFatura.Columns[4].HeaderText = "Katagori";
        }
        private void reLoadFaturaUrunFaturaTableRows()
        {
            this.dgvFaturaUrunFatura.Columns[0].HeaderText = "Fatura ID";
            this.dgvFaturaUrunFatura.Columns[1].HeaderText = "Fatura Tarihi";
            this.dgvFaturaUrunFatura.Columns[2].HeaderText = "Toplam Fiyat";
            this.dgvFaturaUrunFatura.Columns[3].HeaderText = "Müşteri";
            this.dgvFaturaUrunFatura.Columns[4].HeaderText = "Katagori";
        }
        private void reLoadFaturaIadeFaturaTableRows()
        {
            this.dgvFaturaIadeFatura.Columns[0].HeaderText = "Fatura ID";
            this.dgvFaturaIadeFatura.Columns[1].HeaderText = "Fatura Tarihi";
            this.dgvFaturaIadeFatura.Columns[2].HeaderText = "Toplam Fiyat";
            this.dgvFaturaIadeFatura.Columns[3].HeaderText = "Müşteri";
            this.dgvFaturaIadeFatura.Columns[4].HeaderText = "Katagori";
        }
        private void reLoadFaturaMakinaFaturaTableRows()
        {
            this.dgvFaturaMakinaFatura.Columns[0].HeaderText = "Fatura ID";
            this.dgvFaturaMakinaFatura.Columns[1].HeaderText = "Fatura Tarihi";
            this.dgvFaturaMakinaFatura.Columns[2].HeaderText = "Toplam Fiyat";
            this.dgvFaturaMakinaFatura.Columns[3].HeaderText = "Müşteri";
            this.dgvFaturaMakinaFatura.Columns[4].HeaderText = "Katagori";
        }
        private void reLoadFaturaYedekFaturaTableRows()
        {
            this.dgvFaturaYedekFatura.Columns[0].HeaderText = "Fatura ID";
            this.dgvFaturaYedekFatura.Columns[1].HeaderText = "Fatura Tarihi";
            this.dgvFaturaYedekFatura.Columns[2].HeaderText = "Toplam Fiyat";
            this.dgvFaturaYedekFatura.Columns[3].HeaderText = "Müşteri";
            this.dgvFaturaYedekFatura.Columns[4].HeaderText = "Katagori";
        }
        public void reLoadFaturaUrun()
        {
            this.dgvFaturaUrunFatura.Columns[0].HeaderText = "Fatura ID";
            this.dgvFaturaUrunFatura.Columns[1].HeaderText = "Parça ID";
            this.dgvFaturaUrunFatura.Columns[2].HeaderText = "Renk Kodu";
            this.dgvFaturaUrunFatura.Columns[3].HeaderText = "Ürün Adı";
            this.dgvFaturaUrunFatura.Columns[4].HeaderText = "Kalınlık";
            this.dgvFaturaUrunFatura.Columns[5].HeaderText = "Marka";
            this.dgvFaturaUrunFatura.Columns[6].HeaderText = "Fiyat";
            this.dgvFaturaUrunFatura.Columns[7].HeaderText = "Satış Sayısı";
            this.dgvFaturaUrunFatura.Columns[8].HeaderText = "Birim Değeri";
            this.dgvFaturaUrunFatura.Columns[9].HeaderText = "Vade";

            this.dgvFaturaUrunFatura.Columns[1].Visible = false;
        }
        public void reLoadFaturaIade()
        {
            this.dgvFaturaIadeFatura.Columns[0].HeaderText = "Fatura ID";
            this.dgvFaturaIadeFatura.Columns[1].HeaderText = "Parça ID";
            this.dgvFaturaIadeFatura.Columns[2].HeaderText = "Renk Kodu";
            this.dgvFaturaIadeFatura.Columns[3].HeaderText = "Ürün Adı";
            this.dgvFaturaIadeFatura.Columns[4].HeaderText = "Kalınlık";
            this.dgvFaturaIadeFatura.Columns[5].HeaderText = "Marka";
            this.dgvFaturaIadeFatura.Columns[6].HeaderText = "Fiyat";
            this.dgvFaturaIadeFatura.Columns[7].HeaderText = "Satış Sayısı";
            this.dgvFaturaIadeFatura.Columns[8].HeaderText = "Birim Değeri";
            this.dgvFaturaIadeFatura.Columns[9].HeaderText = "Vade";

            this.dgvFaturaIadeFatura.Columns[1].Visible = false;
        }
        public void reLoadFaturaMakina()
        {
            this.dgvFaturaMakinaFatura.Columns[0].HeaderText = "Fatura ID";
            this.dgvFaturaMakinaFatura.Columns[1].HeaderText = "Parça ID";
            this.dgvFaturaMakinaFatura.Columns[2].HeaderText = "Marka";
            this.dgvFaturaMakinaFatura.Columns[3].HeaderText = "Açıklama";
            this.dgvFaturaMakinaFatura.Columns[4].HeaderText = "Satış Adedi";
            this.dgvFaturaMakinaFatura.Columns[5].HeaderText = "Fiyat";
            this.dgvFaturaMakinaFatura.Columns[6].HeaderText = "Toplam Fiyat";
            this.dgvFaturaMakinaFatura.Columns[7].HeaderText = "Firma";
            this.dgvFaturaMakinaFatura.Columns[8].HeaderText = "Firma İletişim";
            this.dgvFaturaMakinaFatura.Columns[9].HeaderText = "Satış Tipi";

            this.dgvFaturaMakinaFatura.Columns[1].Visible = false;
        }
        public void reLoadFaturaYedek()
        {
            this.dgvFaturaYedekFatura.Columns[0].HeaderText = "Fatura ID";
            this.dgvFaturaYedekFatura.Columns[1].HeaderText = "Parça ID";
            this.dgvFaturaYedekFatura.Columns[2].HeaderText = "Marka";
            this.dgvFaturaYedekFatura.Columns[3].HeaderText = "Açıklama";
            this.dgvFaturaYedekFatura.Columns[4].HeaderText = "Satış Adedi";
            this.dgvFaturaYedekFatura.Columns[5].HeaderText = "Fiyat";
            this.dgvFaturaYedekFatura.Columns[6].HeaderText = "Toplam Fiyat";
            this.dgvFaturaYedekFatura.Columns[7].HeaderText = "Firma";
            this.dgvFaturaYedekFatura.Columns[8].HeaderText = "Firma İletişim";
            this.dgvFaturaYedekFatura.Columns[9].HeaderText = "Satış Tipi";

            this.dgvFaturaYedekFatura.Columns[1].Visible = false;
            this.dgvFaturaYedekFatura.Columns[8].Visible = false;
            this.dgvFaturaYedekFatura.Columns[9].Visible = false;
        }
        private void documentFatura_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (tControlFatura.SelectedIndex == tpFaturaUrun.TabIndex)
            {
                var row = dgvFaturaUrunFatura.CurrentRow;
                if (row != null)
                {
                    int invoiceIdInRow = Convert.ToInt32(row.Cells[0].Value);
                    urunSatPage(e, invoiceIdInRow);
                }
            }
            else if (tControlFatura.SelectedIndex == tpFaturaIade.TabIndex)
            {
                var row = dgvFaturaIadeFatura.CurrentRow;
                if (row != null)
                {
                    int invoiceIdInRow = Convert.ToInt32(row.Cells[0].Value);
                    iadePage(e, invoiceIdInRow);
                }
            }
            else if (tControlFatura.SelectedIndex == tpFaturaMakina.TabIndex)
            {
                var row = dgvFaturaMakinaFatura.CurrentRow;
                if (row != null)
                {

                    int invoiceIdInRow = Convert.ToInt32(row.Cells[0].Value);
                    makinaSatPage(e, invoiceIdInRow);

                }
            }
            else if (tControlFatura.SelectedIndex == tpFaturaYedek.TabIndex)
            {
                var row = dgvFaturaYedekFatura.CurrentRow;
                if (row != null)
                {
                    int invoiceIdInRow = Convert.ToInt32(row.Cells[0].Value);
                    yedekSatPage(e, invoiceIdInRow);
                }
            }
        }
        private void getFaturaGetTabIndex()
        {
            this.tpFaturaTum.TabIndex = 0;
            this.tpFaturaUrun.TabIndex = 1;
            this.tpFaturaIade.TabIndex = 2;
            this.tpFaturaMakina.TabIndex = 3;
            this.tpFaturaYedek.TabIndex = 4;
        }
        private void getFaturaPrintCode()
        {
            var ps = new PageSettings();
            var papersizes = new PaperSize("A6", 815, 565);
            ps.PaperSize = papersizes;
            ps.Margins.Top = 0;
            ps.Margins.Bottom = 0;
            ps.Margins.Left = 5;
            ps.Margins.Right = 5;
            documentFatura.DefaultPageSettings = ps;
            documentFatura.Print();
        }
        private void tControlFatura_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == tpFaturaTum.TabIndex)
            {
                loadFatura();
                reLoadFaturaTumFaturaTableRows();
            }
            else if (e.TabPageIndex == tpFaturaUrun.TabIndex)
            {
                loadFaturaUrun();
                reLoadFaturaUrunFaturaTableRows();
            }
            else if (e.TabPageIndex == tpFaturaIade.TabIndex)
            {
                loadFaturaIade();
                reLoadFaturaIadeFaturaTableRows();
            }
            else if (e.TabPageIndex == tpFaturaMakina.TabIndex)
            {
                loadFaturaMakine();
                reLoadFaturaMakinaFaturaTableRows();
            }
            else if (e.TabPageIndex == tpFaturaYedek.TabIndex)
            {
                loadFaturaYedek();
                reLoadFaturaYedekFaturaTableRows();
            }
        }

        private void tbxFaturaTumFaturalarGunSirala_TextChanged_1(object sender, EventArgs e)
        {
            try
            {
                if (tbxFaturaTumFaturalarGunSirala.Text != "")
                {
                    dgvFaturaTumFatura.DataSource =
                        _invoiceService.GetByDay(tbxFaturaTumFaturalarGunSirala.Text);
                }
                else
                {
                    dgvFaturaTumFatura.DataSource = _invoiceServiceList;
                    loadFatura();
                }
            }
            catch
            {
                MessageBox.Show("Sıralama Başarassız!");
            }
        }
        private void tbxFaturaTumFaturalarAySirala_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tbxFaturaTumFaturalarAySirala.Text != "")
                {
                    dgvFaturaTumFatura.DataSource =
                        _invoiceService.GetByMount(tbxFaturaTumFaturalarAySirala.Text);
                }
                else
                {
                    dgvFaturaTumFatura.DataSource = _invoiceServiceList;
                    loadFatura();
                }
            }
            catch
            {
                MessageBox.Show("Sıralama Başarassız!");
            }
        }
        private void tbxFaturaTumFaturalarFirmaSirala_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tbxFaturaTumFaturalarFirmaSirala.Text != "")
                {
                    dgvFaturaTumFatura.DataSource =
                        _invoiceService.GetByCompany(tbxFaturaTumFaturalarFirmaSirala.Text);
                }
                else
                {
                    dgvFaturaTumFatura.DataSource = _invoiceServiceList;
                    loadFatura();
                }
            }
            catch
            {
                MessageBox.Show("Sıralama Başarassız!");
            }
        }
        private void btnFaturaTumFaturalarZRapor_Click(object sender, EventArgs e)
        {

            List<String> textFatura = new List<string>();
            string fileName = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + "Faturalar.txt";
            string faturaString = "";
            string detayString = "";
            string ayrac = "******************************************************************************************************************************************************************************************************";
            textFatura.Clear();
            foreach (DataGridViewRow rows in dgvFaturaTumFatura.Rows)
            {
                if (rows.Cells[4].Value.ToString() == "URUN")
                {
                    textFatura.Add(ayrac);
                    faturaString = "Fatura Tarihi : " + rows.Cells[1].Value.ToString() + "               Kategori = " + rows.Cells[4].Value.ToString() + "               Firma = " + rows.Cells[3].Value.ToString();
                    textFatura.Add(faturaString);
                    foreach (Price price in _priceService.GetByInvoiceId(Convert.ToInt32(rows.Cells[0].Value)))
                    {
                        detayString = "\n{\nÜrün İsmi : " + price.productname + "\nMarka : " + price.productbrand +
                                      "\nRenk Kodu : " + price.colorcode + "\nSatış Adedi : " + convertTT(price.salesquantity.ToString("N4")) +
                                      "\nBirim Fiyatı : " + convertTT(price.productprice.ToString("N4")) + "\nToplam Fiyat : " +
                                      convertTT((price.productprice * price.salesquantity).ToString("N4")) + "\n}\n";
                        textFatura.Add(detayString);
                    }
                }
                else if (rows.Cells[4].Value.ToString() == "IADE")
                {
                    textFatura.Add(ayrac);
                    faturaString = "Fatura Tarihi : " + rows.Cells[1].Value.ToString() + "               Kategori = " + rows.Cells[4].Value.ToString() + "               Firma = " + rows.Cells[3].Value.ToString();
                    textFatura.Add(faturaString);
                    foreach (Price price in _priceService.GetByInvoiceId(Convert.ToInt32(rows.Cells[0].Value)))
                    {
                        detayString = "\n{\nÜrün İsmi : " + price.productname + "\nMarka : " + price.productbrand +
                                      "\nRenk Kodu : " + price.colorcode + "\nSatış Adedi : " + convertTT(price.salesquantity.ToString("N4")) +
                                      "\nBirim Fiyatı : " + convertTT(price.productprice.ToString("N4")) + "\nToplam Fiyat : " +
                                      convertTT((price.productprice * price.salesquantity).ToString("N4")) + "\n}\n";
                        textFatura.Add(detayString);
                    }
                }
                else if (rows.Cells[4].Value.ToString() == "MAKİNA")
                {
                    textFatura.Add(ayrac);
                    faturaString = "Fatura Tarihi : " + rows.Cells[1].Value.ToString() + "               Kategori = " + rows.Cells[4].Value.ToString() + "               Firma = " + rows.Cells[3].Value.ToString();
                    textFatura.Add(faturaString);
                    foreach (Newprice newprice in _newpriceService.GetByInvoiceId(Convert.ToInt32(rows.Cells[0].Value)))
                    {
                        detayString = "\n{\nMarka : " + newprice.npbrand +
                                      "\nAçıklama : " + newprice.npcomment + "\nSatış Adedi : " + convertTT(newprice.npquantity.ToString("N4")) +
                                      "\nBirim Fiyatı : " + convertTT(newprice.nptotalprice.ToString("N4")) + "\nToplam Fiyat : " +
                                      convertTT((newprice.npquantity * newprice.nptotalprice).ToString("N4")) + "\n}\n";
                        textFatura.Add(detayString);
                    }
                }
                else if (rows.Cells[4].Value.ToString() == "YEDEK")
                {
                    textFatura.Add(ayrac);
                    faturaString = "Fatura Tarihi : " + rows.Cells[1].Value.ToString() + "               Kategori = " + rows.Cells[4].Value.ToString() + "               Firma = " + rows.Cells[3].Value.ToString();
                    textFatura.Add(faturaString);
                    foreach (Newprice newprice in _newpriceService.GetByInvoiceId(Convert.ToInt32(rows.Cells[0].Value)))
                    {
                        detayString = "\n{\nMarka : " + newprice.npbrand +
                                      "\nAçıklama : " + newprice.npcomment + "\nSatış Adedi : " + convertTT(newprice.npquantity.ToString("N4")) +
                                      "\nBirim Fiyatı : " + convertTT(newprice.nptotalprice.ToString("N4")) + "\nToplam Fiyat : " +
                                      convertTT((newprice.npquantity * newprice.nptotalprice).ToString("N4")) + "\n}\n";
                        textFatura.Add(detayString);
                    }

                }
            }
            File.WriteAllLines(fileName, textFatura);
            MessageBox.Show("Z Raporu Yazıldı.");
        }

        private void tbxFaturaUrunFaturaAySirala_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tbxFaturaUrunFaturaAySirala.Text != "")
                {
                    dgvFaturaUrunFatura.DataSource =
                        _invoiceService.GetByMount(tbxFaturaUrunFaturaAySirala.Text);
                }
                else
                {
                    dgvFaturaUrunFatura.DataSource = _invoiceServiceListByProduct;
                    loadFaturaUrun();
                }
            }
            catch
            {
                MessageBox.Show("Sıralama Başarassız!");
            }
        }
        private void btnFaturaUrunFaturaGeriDon_Click(object sender, EventArgs e)
        {
            dgvFaturaUrunFatura.DataSource = _invoiceServiceListByProduct;
            loadFaturaUrun();
            reLoadFaturaUrunFaturaTableRows();
        }
        private void btnFaturaUrunFaturaYazdir_Click(object sender, EventArgs e)
        {
            getFaturaPrintCode();
        }
        private void dgvFaturaUrunFatura_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvFaturaUrunFatura.CurrentRow;
            if (row != null)
            {
                dgvFaturaUrunFatura.DataSource = _priceService.GetByInvoiceId(Convert.ToInt32(row.Cells[0].Value));
                reLoadFaturaUrun();
            }
        }

        private void tbxFaturaIadeFaturaAySirala_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tbxFaturaIadeFaturaAySirala.Text != "")
                {
                    dgvFaturaIadeFatura.DataSource =
                        _invoiceService.GetByMount(tbxFaturaIadeFaturaAySirala.Text);
                }
                else
                {
                    dgvFaturaIadeFatura.DataSource = _invoiceServiceListByReturn;
                    loadFaturaIade();
                }
            }
            catch
            {
                MessageBox.Show("Sıralama Başarassız!");
            }
        }
        private void btnFaturaIadeFaturaGeriDon_Click(object sender, EventArgs e)
        {
            dgvFaturaIadeFatura.DataSource = _invoiceServiceListByReturn;
            loadFaturaIade();
            reLoadFaturaIadeFaturaTableRows();
        }
        private void btnFaturaIadeFaturaYazdir_Click(object sender, EventArgs e)
        {
            getFaturaPrintCode();
        }
        private void dgvFaturaIadeFatura_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvFaturaIadeFatura.CurrentRow;
            if (row != null)
            {
                dgvFaturaIadeFatura.DataSource = _priceService.GetByInvoiceId(Convert.ToInt32(row.Cells[0].Value));
                reLoadFaturaIade();
            }
        }

        private void tbxFaturaMakinaFaturaAySirala_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tbxFaturaMakinaFaturaAySirala.Text != "")
                {
                    dgvFaturaMakinaFatura.DataSource =
                        _invoiceService.GetByMount(tbxFaturaMakinaFaturaAySirala.Text);
                }
                else
                {
                    dgvFaturaMakinaFatura.DataSource = _invoiceServiceListByMachine;
                    loadFaturaIade();
                }
            }
            catch
            {
                MessageBox.Show("Sıralama Başarassız!");
            }
        }
        private void btnFaturaMakinaFaturaGeriDon_Click(object sender, EventArgs e)
        {
            dgvFaturaMakinaFatura.DataSource = _invoiceServiceListByMachine;
            loadFaturaMakine();
            reLoadFaturaMakinaFaturaTableRows();
        }
        private void btnFaturaMakinaFaturaYazdir_Click(object sender, EventArgs e)
        {
            getFaturaPrintCode();
        }
        private void dgvFaturaMakinaFatura_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvFaturaMakinaFatura.CurrentRow;
            if (row != null)
            {
                dgvFaturaMakinaFatura.DataSource = _newpriceService.GetByInvoiceId(Convert.ToInt32(row.Cells[0].Value));
                reLoadFaturaMakina();
            }
        }

        private void cbxFaturaYedekFaturaAySirala_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tbxFaturaYedekFaturaAySirala.Text != "")
                {
                    dgvFaturaYedekFatura.DataSource =
                        _invoiceService.GetByMount(tbxFaturaYedekFaturaAySirala.Text);
                }
                else
                {
                    dgvFaturaYedekFatura.DataSource = _invoiceServiceListBySparePart;
                    loadFaturaIade();
                }
            }
            catch
            {
                MessageBox.Show("Sıralama Başarassız!");
            }
        }
        private void btnFaturaYedekFaturaGeriDon_Click(object sender, EventArgs e)
        {
            dgvFaturaYedekFatura.DataSource = _invoiceServiceListBySparePart;
            loadFaturaYedek();
            reLoadFaturaYedekFaturaTableRows();
        }
        private void btnFaturaYedekFaturaYazdir_Click(object sender, EventArgs e)
        {
            getFaturaPrintCode();
        }
        private void dgvFaturaYedekFatura_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvFaturaYedekFatura.CurrentRow;
            if (row != null)
            {
                dgvFaturaYedekFatura.DataSource = _newpriceService.GetByInvoiceId(Convert.ToInt32(row.Cells[0].Value));
                reLoadFaturaYedek();
            }
        }
        #endregion

        #region Ayarlar

        private void getAyarlar()
        {
            getAyarlarGetTabIndex();
            loadAyarlarMarka();
        }
        private void tControlAyarlar_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPageIndex == tpAyarlarMarka.TabIndex)
            {
                loadAyarlarMarka();
                tbxAyarlarKalinlikGuncelle.Clear();
                tbxAyarlarBirimGuncelle.Clear();
                dgvAyarlar.ClearSelection();
            }
            else if (e.TabPageIndex == tpAyarlarKalinlik.TabIndex)
            {
                loadAyarlarKalinlik();
                tbxAyarlarMarkaGuncelle.Clear();
                tbxAyarlarBirimGuncelle.Clear();
                dgvAyarlar.ClearSelection();
            }
            else if (e.TabPageIndex == tpAyarlarBirim.TabIndex)
            {
                loadAyarlarBirim();
                tbxAyarlarMarkaGuncelle.Clear();
                tbxAyarlarKalinlikGuncelle.Clear();
                dgvAyarlar.ClearSelection();
            }
        }
        private void getAyarlarGetTabIndex()
        {
            this.tpAyarlarMarka.TabIndex = 0;
            this.tpAyarlarKalinlik.TabIndex = 1;
            this.tpAyarlarBirim.TabIndex = 2;
        }
        private void loadAyarlarMarka()
        {
            _brandServicesList = _brandServices.GetAll();

            dgvAyarlar.DataSource = _brandServicesList;
            dgvAyarlar.Columns[0].HeaderText = "Marka Id";
            dgvAyarlar.Columns[1].HeaderText = "Marka";
            dgvAyarlar.Columns[0].Visible = false;
        }
        private void loadAyarlarKalinlik()
        {
            _thicknessServicesList = _thicknessServices.GetAll();

            dgvAyarlar.DataSource = _thicknessServicesList;
            dgvAyarlar.Columns[0].HeaderText = "Kalınlık Id";
            dgvAyarlar.Columns[1].HeaderText = "Kalınlık";
            dgvAyarlar.Columns[0].Visible = false;
        }
        private void loadAyarlarBirim()
        {
            _unitServicesList = _unitServices.GetAll();

            dgvAyarlar.DataSource = _unitServicesList;
            dgvAyarlar.Columns[0].HeaderText = "Birim Id";
            dgvAyarlar.Columns[1].HeaderText = "Birim";
            dgvAyarlar.Columns[0].Visible = false;
        }

        private void btnAyarlarMarkaEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxAyarlarMarkaEkle.Text == "")
                {
                    MessageBox.Show("Marka Giriniz");
                }
                else if (_brandServices.Get(tbxAyarlarMarkaEkle.Text) != null)
                {
                    MessageBox.Show("Böyle Bir Marka Var");
                }
                else
                {
                    btnAyarlarMarkaEkle.Enabled = false;
                    _brandServices.Add(new Brand
                    {
                        brandname = tbxAyarlarMarkaEkle.Text
                    });
                    tbxAyarlarMarkaEkle.Clear();

                    loadAyarlarMarka();
                    btnAyarlarMarkaEkle.Enabled = true;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnAyarlarMarkaGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                var row = dgvAyarlar.CurrentRow;
                if (row != null)
                {
                    if (tbxAyarlarMarkaGuncelle.Text == "")
                    {
                        MessageBox.Show("Marka Giriniz");
                    }
                    else
                    {
                        btnAyarlarMarkaGuncelle.Enabled = false;
                        _brandServices.Update(new Brand
                        {
                            brandid = Convert.ToInt32(row.Cells[0].Value),
                            brandname = tbxAyarlarMarkaGuncelle.Text
                        });
                        tbxAyarlarMarkaGuncelle.Clear();

                        loadAyarlarMarka();
                        btnAyarlarMarkaGuncelle.Enabled = true;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnAyarlarMarkaSil_Click(object sender, EventArgs e)
        {
            try
            {
                var row = dgvAyarlar.CurrentRow;
                if (row != null)
                {
                    btnAyarlarMarkaSil.Enabled = false;
                    _brandServices.Delete(new Brand
                    {
                        brandid = Convert.ToInt32(row.Cells[0].Value)
                    });
                    tbxAyarlarMarkaGuncelle.Clear();

                    loadAyarlarMarka();
                    btnAyarlarMarkaSil.Enabled = true;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnAyarlarKalinlikEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxAyarlarKalinlikEkle.Text == "")
                {
                    MessageBox.Show("Kalınlık Giriniz");
                }
                else if (_thicknessServices.Get(tbxAyarlarKalinlikEkle.Text) != null)
                {
                    MessageBox.Show("Böyle Bir Kalınlık Var");
                }
                else
                {
                    btnAyarlarKalinlikEkle.Enabled = false;
                    _thicknessServices.Add(new Thickness
                    {
                        thicknessname = tbxAyarlarKalinlikEkle.Text
                    });
                    tbxAyarlarKalinlikEkle.Clear();

                    loadAyarlarKalinlik();
                    btnAyarlarKalinlikEkle.Enabled = true;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnAyarlarKalinlikGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                var row = dgvAyarlar.CurrentRow;
                if (row != null)
                {
                
                    if (tbxAyarlarKalinlikGuncelle.Text == "")
                    {
                        MessageBox.Show("Kalınlık Giriniz");
                    }
                    else
                    {
                        btnAyarlarKalinlikGuncelle.Enabled = false;
                        _thicknessServices.Update(new Thickness
                        {
                            thicknessid = Convert.ToInt32(row.Cells[0].Value),
                            thicknessname = tbxAyarlarKalinlikGuncelle.Text
                        });
                        tbxAyarlarKalinlikGuncelle.Clear();

                        loadAyarlarKalinlik();
                        btnAyarlarKalinlikGuncelle.Enabled = true;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnAyarlarKalinlikSil_Click(object sender, EventArgs e)
        {
            try
            {
                var row = dgvAyarlar.CurrentRow;
                if (row != null)
                {
                    btnAyarlarKalinlikSil.Enabled = false;
                    _thicknessServices.Delete(new Thickness
                    {
                        thicknessid = Convert.ToInt32(row.Cells[0].Value),
                    });
                    tbxAyarlarKalinlikGuncelle.Clear();

                    loadAyarlarKalinlik();
                    btnAyarlarKalinlikSil.Enabled = true;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void btnAyarlarBirimEkle_Click(object sender, EventArgs e)
        {
            try
            {
                if (tbxAyarlarBirimEkle.Text == "")
                {
                    MessageBox.Show("Birim Değeri Giriniz");
                }
                else if (_unitServices.Get(tbxAyarlarBirimEkle.Text) != null)
                {
                    MessageBox.Show("Böyle Bir Birim Var");
                }
                else
                {
                    btnAyarlarBirimEkle.Enabled = false;
                    _unitServices.Add(new Unit
                    {
                        unitname = tbxAyarlarBirimEkle.Text
                    });
                    tbxAyarlarBirimEkle.Clear();

                    loadAyarlarBirim();
                    btnAyarlarBirimEkle.Enabled = true;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnAyarlarBirimGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                var row = dgvAyarlar.CurrentRow;
                if (row != null)
                {
                    if (tbxAyarlarBirimGuncelle.Text == "")
                    {
                        MessageBox.Show("Birim Değeri Giriniz");
                    }
                    else
                    {
                        btnAyarlarBirimGuncelle.Enabled = false;
                        _unitServices.Update(new Unit
                        {
                            unitid = Convert.ToInt32(row.Cells[0].Value),
                            unitname = tbxAyarlarBirimGuncelle.Text
                        });
                        tbxAyarlarBirimGuncelle.Clear();

                        loadAyarlarBirim();
                        btnAyarlarBirimGuncelle.Enabled = true;
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }
        private void btnAyarlarBirimSil_Click(object sender, EventArgs e)
        {
            try
            {
                var row = dgvAyarlar.CurrentRow;
                if (row != null)
                {
                    btnAyarlarBirimSil.Enabled = false;
                    _unitServices.Delete(new Unit
                    {
                        unitid = Convert.ToInt32(row.Cells[0].Value),
                    });
                    tbxAyarlarBirimGuncelle.Clear();

                    loadAyarlarBirim();
                    btnAyarlarBirimSil.Enabled = true;
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void dgvAyarlar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvAyarlar.CurrentRow;
            if (row != null)
            {
                if (tpAyarlarMarka.TabIndex == tControlAyarlar.SelectedIndex)
                {
                    tbxAyarlarMarkaGuncelle.Text = row.Cells[1].Value.ToString();
                    tbxAyarlarKalinlikGuncelle.Clear();
                    tbxAyarlarBirimGuncelle.Clear();
                }
                else if (tpAyarlarKalinlik.TabIndex == tControlAyarlar.SelectedIndex)
                {
                    tbxAyarlarKalinlikGuncelle.Text = row.Cells[1].Value.ToString();
                    tbxAyarlarMarkaGuncelle.Clear();
                    tbxAyarlarBirimGuncelle.Clear();
                }
                else if (tpAyarlarBirim.TabIndex == tControlAyarlar.SelectedIndex)
                {
                    tbxAyarlarBirimGuncelle.Text = row.Cells[1].Value.ToString();
                    tbxAyarlarMarkaGuncelle.Clear();
                    tbxAyarlarKalinlikGuncelle.Clear();
                }
            }
        }

        #endregion

    }
}
