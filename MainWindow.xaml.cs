//******************************************************
// File: MainWindow.xaml.cs
//
// Purpose: To establish the data context of the xaml GUI
// and house the code for the many processes of the GUI
//
// Written By: Jonathon Carrera
//
// Compiler: Visual Studio 2019
//
//******************************************************

using Microsoft.Win32;
using Publishing_Solution;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
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

namespace Publisher_GUI_2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BookPublishingEntities bookPublishingEntities;
        private Publisher publisher;
        public MainWindow()
        {
            InitializeComponent();

            bookPublishingEntities = new BookPublishingEntities();
            publisher = new Publisher();

            bookPublishingEntities.Books.Load();
            this.DataContext = bookPublishingEntities.Books.Local;
        }

        #region Methods
        //****************************************************
        // Method: closeApp
        //
        // Purpose: To close the program
        //****************************************************
        public void closeApp(object sender, RoutedEventArgs e)
        {
            // https://www.c-sharpcorner.com/UploadFile/18ddf7/different-methods-to-close-a-wpf-application/
            App.Current.Shutdown();
        }

        //****************************************************
        // Method: importFromJson
        //
        // Purpose: To carry out the funtionality of the
        // "Import Publisher From JSON File" menu item by calling
        // the appropriate methods in order. 
        //****************************************************
        public void importFromJSON(object sender, RoutedEventArgs e)
        {
            // Could I have used a command to run all of these methods instead?
            openDialog();
            clearDB();
            updateDB();
        }

        //****************************************************
        // Method: openDialog
        //
        // Purpose: To open a file dialog so that a user can
        // select a JSON file to read in an instance of publisher
        // and save it to a local instance in the program
        //****************************************************
        public void openDialog()
        {
            // Note:All this code was taken from HW3 that's why no comments on lines

            string filename = "";

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open Publisher From JSON";
            openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.FileName;
                FileStream reader = new FileStream(filename, FileMode.Open, FileAccess.Read);
                DataContractJsonSerializer inputSerializer;
                inputSerializer = new DataContractJsonSerializer(typeof(Publisher));

                publisher = (Publisher)inputSerializer.ReadObject(reader);
                reader.Dispose();
            }
        }

        //****************************************************
        // Method: clearDB
        //
        // Purpose: To remove all the data from the SQL database,
        // save the changes and reset the auto incremented
        // Id property to 0.
        //****************************************************
        public void clearDB() 
        {
            // Got these 2 directly from the slides provided in class
            // This one removes the data from the table (the local one I think)
            bookPublishingEntities.Books.RemoveRange(bookPublishingEntities.Books);
            // This one commits the changes to actual SQL database 
            bookPublishingEntities.SaveChanges();

            //**************************************************** 
            // Kinda proud of this one while I was googling how to 
            // Reset the auto incremented Id property I came across
            // this link:
            //
            // https://www.mysqltutorial.org/mysql-reset-auto-increment/#:~:text=The%20syntax%20of%20the%20ALTER,in%20the%20expression%20AUTO_INCREMENT%3Dvalue%20.
            //
            // which just lists some of the ways the operation can be 
            // done using SQL command than I remembered a method called 
            // ExecuteSqlCommand in the slides and it clicked. 
            //**************************************************** 
            bookPublishingEntities.Database.ExecuteSqlCommand("TRUNCATE TABLE BOOKS");
        }

        //****************************************************
        // Method: updateDB
        //
        // Purpose: To use an instance of publisher to fill in
        // the Books table of my SQL database.
        //****************************************************
        public void updateDB() 
        {
            //**************************************************** 
            // Realized that in this program there exist two
            // definitions of the Book class and that within this 
            // namespace it will always default to the Entity frameworks
            // version. So I remembered that in C++ we learned about
            // using different namespaces to refer to different 
            // classes and methods that have the same names.
            //**************************************************** 
            foreach (Publishing_Solution.Book b in publisher.Books)
            {
                //**************************************************** 
                // At first I tried storing the data of the books inside
                // a local variable but for some reason it would only add
                // the last book in the Books list, changed it to this
                // and it worked just fine. I like this way better but
                // I'm super confused as to why my first attempt didn't 
                // work
                //**************************************************** 
                bookPublishingEntities.Books.Add(new Book { Title = b.Title, Price = b.Price });
            }

            // Commits changes from the local table to the real one in the data base
            bookPublishingEntities.SaveChanges();
        }

        //****************************************************
        // Method: showAbout
        //
        // Purpose: To display a message box that contains the
        // name of the application, the version number and the
        // name of the author
        //****************************************************
        public void showAbout(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Book Publishing GUI\n" + "Version 2.0\n" + "Written by Jonathon Carrera", "About Book Publishing GUI");
        }
        #endregion
    }
}
