using AutoLotModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Pojar_razvan_proiect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    enum ActionState
    {
        New,
        Edit,
        Delete,
        Nothing
    }
    public partial class MainWindow : Window
    {
        ActionState action = ActionState.Nothing;
        AutoLotEntitiesModel ctx = new AutoLotEntitiesModel();

        CollectionViewSource clientViewSource;
        Binding txtNume_PrenumeBinding = new Binding();
        Binding txtEmailBinding = new Binding();

        CollectionViewSource masinaViewSource;
        Binding txtMarcaBinding = new Binding();
        Binding txtModelBinding = new Binding();

        CollectionViewSource clientOrdersViewSource;
        Binding txtClient = new Binding();
        Binding txtMasina = new Binding();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            txtNume_PrenumeBinding.Path = new PropertyPath("Nume_Prenume");
            txtEmailBinding.Path = new PropertyPath("Email");
            txtMarcaBinding.Path = new PropertyPath("Marca");
            txtModelBinding.Path = new PropertyPath("Model");
            txtClient.Path = new PropertyPath("Client");
            txtMasina.Path = new PropertyPath("Masina");

            nume_PrenumeTextBox.SetBinding(TextBox.TextProperty, txtNume_PrenumeBinding);
            emailTextBox.SetBinding(TextBox.TextProperty, txtEmailBinding);
            marcaTextBox.SetBinding(TextBox.TextProperty, txtMarcaBinding);
            modelTextBox.SetBinding(TextBox.TextProperty, txtModelBinding);
            cmbClient.SetBinding(ComboBox.TextProperty, txtClient);
            cmbMasina.SetBinding(ComboBox.TextProperty, txtMasina);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            clientViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientViewSource")));
            clientViewSource.Source = ctx.Clients.Local;
            clientOrdersViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("clientOrdersViewSource")));
            clientOrdersViewSource.Source = ctx.Orders.Local;
            ctx.Clients.Load();
            ctx.Orders.Load();
            ctx.Masinas.Load();

            cmbClient.ItemsSource = ctx.Clients.Local;
            //cmbClient.DisplayMemberPath = "Nume_Prenume";
            cmbClient.SelectedValuePath = "ClientId";

            cmbMasina.ItemsSource = ctx.Masinas.Local;
            //cmbMasina.DisplayMemberPath = "Marca";
            cmbMasina.SelectedValuePath = "MasinaId";

            System.Windows.Data.CollectionViewSource masinaViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("masinaViewSource")));
            masinaViewSource.Source = ctx.Masinas.Local;
            ctx.Masinas.Load();

            BindDataGrid();
        }
        private void BindDataGrid()
        {
            var queryOrder = from ord in ctx.Orders
                             join cli in ctx.Clients on ord.ClientId equals
                             cli.ClientId
                             join mas in ctx.Masinas on ord.MasinaId
                 equals mas.MasinaId
                             select new
                             {
                                 ord.OrderId,
                                 ord.MasinaId,
                                 ord.ClientId,
                                 cli.Nume_Prenume,
                                 cli.Email,
                                 mas.Model,
                                 mas.Marca
                             };
            clientOrdersViewSource.Source = queryOrder.ToList();

        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Client client = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    client = new Client()
                    {
                        Nume_Prenume = nume_PrenumeTextBox.Text.Trim(),
                        Email = emailTextBox.Text.Trim()
                    };
                    //adaugam entitatea nou creata in context
                    ctx.Clients.Add(client);
                    clientViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                clientDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                nume_PrenumeTextBox.IsEnabled = false;
                emailTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Edit)
            {
                try
                {
                    client = (Client)clientDataGrid.SelectedItem;
                    client.Nume_Prenume = nume_PrenumeTextBox.Text.Trim();
                    client.Email = emailTextBox.Text.Trim();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                clientViewSource.View.Refresh();
                // pozitionarea pe item-ul curent
                clientViewSource.View.MoveCurrentTo(client);

                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                clientDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;

                nume_PrenumeTextBox.IsEnabled = false;
                emailTextBox.IsEnabled = false;
            }

            else if (action == ActionState.Delete)
            {
                try
                {
                    client = (Client)clientDataGrid.SelectedItem;
                    ctx.Clients.Remove(client);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                clientViewSource.View.Refresh();
                btnNew.IsEnabled = true;
                btnEdit.IsEnabled = true;
                btnDelete.IsEnabled = true;
                btnSave.IsEnabled = false;
                btnCancel.IsEnabled = false;
                clientDataGrid.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;

                nume_PrenumeTextBox.IsEnabled = false;
                emailTextBox.IsEnabled = false;

                nume_PrenumeTextBox.SetBinding(TextBox.TextProperty, txtNume_PrenumeBinding);
                emailTextBox.SetBinding(TextBox.TextProperty, txtEmailBinding);
            }
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            clientViewSource.View.MoveCurrentToNext();
        }
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            clientViewSource.View.MoveCurrentToPrevious();
        }
        private void btnNew_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;

            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            clientDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;

            nume_PrenumeTextBox.IsEnabled = true;
            emailTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(nume_PrenumeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(emailTextBox, TextBox.TextProperty);
            nume_PrenumeTextBox.Text = "";
            emailTextBox.Text = "";
            Keyboard.Focus(nume_PrenumeTextBox);
        }
        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempNume_Prenume = nume_PrenumeTextBox.Text.ToString();
            string tempEmail = emailTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            clientDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;

            nume_PrenumeTextBox.IsEnabled = true;
            emailTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(nume_PrenumeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(emailTextBox, TextBox.TextProperty);
            nume_PrenumeTextBox.Text = tempNume_Prenume;
            emailTextBox.Text = tempEmail;
            Keyboard.Focus(nume_PrenumeTextBox);
        }
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempNume_Prenume = nume_PrenumeTextBox.Text.ToString();
            string tempEmail = emailTextBox.Text.ToString();

            btnNew.IsEnabled = false;
            btnEdit.IsEnabled = false;
            btnDelete.IsEnabled = false;
            btnSave.IsEnabled = true;
            btnCancel.IsEnabled = true;
            clientDataGrid.IsEnabled = false;
            btnPrev.IsEnabled = false;
            btnNext.IsEnabled = false;

            BindingOperations.ClearBinding(nume_PrenumeTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(emailTextBox, TextBox.TextProperty);
            nume_PrenumeTextBox.Text = tempNume_Prenume;
            emailTextBox.Text = tempEmail;
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnEdit.IsEnabled = true;
            btnSave.IsEnabled = false;
            btnCancel.IsEnabled = false;
            clientDataGrid.IsEnabled = true;
            btnPrev.IsEnabled = true;
            btnNext.IsEnabled = true;

            nume_PrenumeTextBox.IsEnabled = false;
            emailTextBox.IsEnabled = false;

            nume_PrenumeTextBox.SetBinding(TextBox.TextProperty, txtNume_PrenumeBinding);
            emailTextBox.SetBinding(TextBox.TextProperty, txtEmailBinding);
        }
        private void btnNew1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;

            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            masinaDataGrid.IsEnabled = false;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;

            marcaTextBox.IsEnabled = true;
            modelTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(marcaTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(modelTextBox, TextBox.TextProperty);
            marcaTextBox.Text = "";
            modelTextBox.Text = "";
            Keyboard.Focus(marcaTextBox);
        }
        private void btnEdit1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempMarca = marcaTextBox.Text.ToString();
            string tempModel = modelTextBox.Text.ToString();

            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;

            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            masinaDataGrid.IsEnabled = false;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;

            marcaTextBox.IsEnabled = true;
            modelTextBox.IsEnabled = true;

            BindingOperations.ClearBinding(marcaTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(modelTextBox, TextBox.TextProperty);
            marcaTextBox.Text = tempMarca;
            modelTextBox.Text = tempModel;
            Keyboard.Focus(marcaTextBox);
        }
        private void btnDelete1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempMarca = marcaTextBox.Text.ToString();
            string tempModel = modelTextBox.Text.ToString();

            btnNew1.IsEnabled = false;
            btnEdit1.IsEnabled = false;
            btnDelete1.IsEnabled = false;

            btnSave1.IsEnabled = true;
            btnCancel1.IsEnabled = true;
            masinaDataGrid.IsEnabled = false;
            btnPrev1.IsEnabled = false;
            btnNext1.IsEnabled = false;

            BindingOperations.ClearBinding(marcaTextBox, TextBox.TextProperty);
            BindingOperations.ClearBinding(modelTextBox, TextBox.TextProperty);
            marcaTextBox.Text = tempMarca;
            modelTextBox.Text = tempModel;
        }
        private void btnSave1_Click(object sender, RoutedEventArgs e)
        {
            Masina masina = null;
            if (action == ActionState.New)
            {
                try
                {
                    //instantiem Customer entity
                    masina = new Masina()
                    {
                        Marca = marcaTextBox.Text.Trim(),
                        Model = modelTextBox.Text.Trim()
                    };

                    //adaugam entitatea nou creata in context
                    ctx.Masinas.Add(masina);
                    masinaViewSource.View.Refresh();
                    //salvam modificarile
                    ctx.SaveChanges();
                }
                //using System.Data;
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                masinaDataGrid.IsEnabled = true;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;
                marcaTextBox.IsEnabled = false;
                modelTextBox.IsEnabled = false;


            }

            else if (action == ActionState.Edit)
            {
                try
                {
                    masina = (Masina)masinaDataGrid.SelectedItem;
                    masina.Marca = marcaTextBox.Text.Trim();
                    masina.Model = modelTextBox.Text.Trim();
                    //salvam modif
                    ctx.SaveChanges();

                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                masinaViewSource.View.Refresh();
                //pozitionarea pe item-ul curent
                masinaViewSource.View.MoveCurrentTo(masina);
                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnDelete1.IsEnabled = true;

                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                masinaDataGrid.IsEnabled = true;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;

                marcaTextBox.IsEnabled = false;
                modelTextBox.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    masina = (Masina)masinaDataGrid.SelectedItem;
                    ctx.Masinas.Remove(masina);
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                masinaViewSource.View.Refresh();

                btnNew1.IsEnabled = true;
                btnEdit1.IsEnabled = true;
                btnDelete1.IsEnabled = true;

                btnSave1.IsEnabled = false;
                btnCancel1.IsEnabled = false;
                masinaDataGrid.IsEnabled = true;
                btnPrev1.IsEnabled = true;
                btnNext1.IsEnabled = true;

                marcaTextBox.IsEnabled = false;
                modelTextBox.IsEnabled = false;

                marcaTextBox.SetBinding(TextBox.TextProperty, txtMarcaBinding);
                modelTextBox.SetBinding(TextBox.TextProperty, txtModelBinding);
            }
        }
        private void btnCancel1_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;
            btnNew1.IsEnabled = true;
            btnEdit1.IsEnabled = true;


            btnSave1.IsEnabled = false;
            btnCancel1.IsEnabled = false;
            masinaDataGrid.IsEnabled = true;
            btnPrev1.IsEnabled = true;
            btnNext1.IsEnabled = true;

            marcaTextBox.IsEnabled = false;
            modelTextBox.IsEnabled = false;

            marcaTextBox.SetBinding(TextBox.TextProperty, txtMarcaBinding);
            modelTextBox.SetBinding(TextBox.TextProperty, txtModelBinding);

        }
        private void btnPrevious1_Click(object sender, RoutedEventArgs e)
        {
            masinaViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext1_Click(object sender, RoutedEventArgs e)
        {
            masinaViewSource.View.MoveCurrentToNext();
        }

        private void btnNew2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.New;
            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;

            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            orderDataGrid.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;

            cmbClient.IsEnabled = true;
            cmbMasina.IsEnabled = true;

            BindingOperations.ClearBinding(cmbClient, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbMasina, ComboBox.TextProperty);
            cmbClient.Text = "";
            cmbMasina.Text = "";
            Keyboard.Focus(cmbClient);
        }

        private void btnEdit2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Edit;

            string tempClient = cmbClient.Text.ToString();
            string tempMasina = cmbMasina.Text.ToString();

            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;

            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            orderDataGrid.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;
            cmbClient.IsEnabled = true;
            cmbMasina.IsEnabled = true;

            BindingOperations.ClearBinding(cmbClient, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbMasina, ComboBox.TextProperty);
            cmbClient.Text = tempClient;
            cmbMasina.Text = tempMasina;
            Keyboard.Focus(cmbClient);
        }
        private void btnDelete2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Delete;

            string tempClient = cmbClient.Text.ToString();
            string tempMasina = cmbMasina.Text.ToString();

            btnNew2.IsEnabled = false;
            btnEdit2.IsEnabled = false;
            btnDelete2.IsEnabled = false;

            btnSave2.IsEnabled = true;
            btnCancel2.IsEnabled = true;
            orderDataGrid.IsEnabled = false;
            btnPrev2.IsEnabled = false;
            btnNext2.IsEnabled = false;


            BindingOperations.ClearBinding(cmbClient, ComboBox.TextProperty);
            BindingOperations.ClearBinding(cmbMasina, ComboBox.TextProperty);
            cmbClient.Text = tempClient;
            cmbMasina.Text = tempMasina;

        }
        private void btnSave2_Click(object sender, RoutedEventArgs e)
        {
            Order order = null;
            if (action == ActionState.New)
            {
                try
                {
                    Client client = (Client)cmbClient.SelectedItem;
                    Masina masina = (Masina)cmbMasina.SelectedItem;

                    //instantiem Order entity
                    order = new Order()
                    {
                        ClientId = client.ClientId,
                        MasinaId = masina.MasinaId

                    };
                    //adaugam entitatea nou creata in context
                    ctx.Orders.Add(order);
                    clientOrdersViewSource.View.Refresh();
                    //salvam modif
                    ctx.SaveChanges();
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                orderDataGrid.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                cmbClient.IsEnabled = false;
                cmbMasina.IsEnabled = false;

            }
            else if (action == ActionState.Edit)
            {
                dynamic selectedOrder = orderDataGrid.SelectedItem;
                try
                {
                    int curr_id = selectedOrder.OrderId;

                    var editedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (editedOrder != null)
                    {
                        editedOrder.ClientId = Int32.Parse(cmbClient.SelectedValue.ToString());
                        editedOrder.MasinaId = Int32.Parse(cmbMasina.SelectedValue.ToString());
                        ctx.SaveChanges();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                BindDataGrid();
                clientViewSource.View.Refresh();
                clientOrdersViewSource.View.MoveCurrentTo(order);
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;

                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                orderDataGrid.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                cmbClient.IsEnabled = false;
                cmbMasina.IsEnabled = false;
            }
            else if (action == ActionState.Delete)
            {
                try
                {
                    dynamic selectedOrder = orderDataGrid.SelectedItem;

                    int curr_id = selectedOrder.OrderId;
                    var deletedOrder = ctx.Orders.FirstOrDefault(s => s.OrderId == curr_id);
                    if (deletedOrder != null)
                    {
                        ctx.Orders.Remove(deletedOrder);
                        ctx.SaveChanges();
                        MessageBox.Show("Order Deleted Successfully", "Message");
                        BindDataGrid();
                    }
                }
                catch (DataException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                clientOrdersViewSource.View.Refresh();
                btnNew2.IsEnabled = true;
                btnEdit2.IsEnabled = true;
                btnDelete2.IsEnabled = true;

                btnSave2.IsEnabled = false;
                btnCancel2.IsEnabled = false;
                orderDataGrid.IsEnabled = true;
                btnPrev2.IsEnabled = true;
                btnNext2.IsEnabled = true;
                cmbClient.IsEnabled = false;
                cmbMasina.IsEnabled = false;

                cmbClient.SetBinding(ComboBox.TextProperty, txtClient);
                cmbMasina.SetBinding(ComboBox.TextProperty, txtMasina);
            }

        }
        private void btnCancel2_Click(object sender, RoutedEventArgs e)
        {
            action = ActionState.Nothing;

            btnNew2.IsEnabled = true;
            btnEdit2.IsEnabled = true;
            btnSave2.IsEnabled = false;
            btnCancel2.IsEnabled = false;
            orderDataGrid.IsEnabled = true;
            btnPrev2.IsEnabled = true;
            btnNext2.IsEnabled = true;

            cmbClient.IsEnabled = false;
            cmbMasina.IsEnabled = false;

            cmbClient.SetBinding(ComboBox.TextProperty, txtClient);
            cmbMasina.SetBinding(ComboBox.TextProperty, txtMasina);
        }
        private void btnPrevious2_Click(object sender, RoutedEventArgs e)
        {
            clientOrdersViewSource.View.MoveCurrentToPrevious();
        }

        private void btnNext2_Click(object sender, RoutedEventArgs e)
        {
            clientOrdersViewSource.View.MoveCurrentToNext();
        }
        private void SetValidationBinding()
        {
            Binding nume_PrenumeValidationBinding = new Binding();
            nume_PrenumeValidationBinding.Source = clientViewSource;
            nume_PrenumeValidationBinding.Path = new PropertyPath("Nume_Prenume");
            nume_PrenumeValidationBinding.NotifyOnValidationError = true;
            nume_PrenumeValidationBinding.Mode = BindingMode.TwoWay;
            nume_PrenumeValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
            //string required
            nume_PrenumeValidationBinding.ValidationRules.Add(new StringNotEmpty());
            nume_PrenumeTextBox.SetBinding(TextBox.TextProperty,
           nume_PrenumeValidationBinding);
            Binding emailValidationBinding = new Binding();
            emailValidationBinding.Source = clientViewSource;
            emailValidationBinding.Path = new PropertyPath("Email");
            emailValidationBinding.NotifyOnValidationError = true;
            emailValidationBinding.Mode = BindingMode.TwoWay;
            emailValidationBinding.UpdateSourceTrigger =
           UpdateSourceTrigger.PropertyChanged;
            //string min length validator
            emailValidationBinding.ValidationRules.Add(new StringMinLengthValidator());
            emailTextBox.SetBinding(TextBox.TextProperty,
           emailValidationBinding); //setare binding nou
        }


    }
}