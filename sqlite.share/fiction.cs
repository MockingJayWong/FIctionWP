using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqlite.share
{
    public class fiction: INotifyPropertyChanged
    {
        [sqlite.share.PrimaryKey, sqlite.share.AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }

        public string Website { get; set; }
        private string newchapter;


        public string Newchapter { get{ return newchapter; }
            set { if (newchapter == value) return;
                newchapter = value;
                SendPropertyChanged("Newchapter");
            } }
        public fiction() { }
        public fiction(string n, string w)
        {
            this.Name = n;
            this.Website = w;
            this.Newchapter = "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SendPropertyChanged(string propertyName)

        {
            if (null != PropertyChanged) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        //public string Name
        //{
        //    get
        //    {
        //        return name;
        //    }
        //    set
        //    {
        //        if (value == name) { return; }
        //        name = value;
        //        NotifyPropertyChanged("Name");
        //    }
        //}



        //public string Newchapter
        //{
        //    get
        //    {
        //        return newchapter;
        //    }
        //    set
        //    {
        //        if (value == newchapter) { return; }
        //        newchapter = value;
        //        NotifyPropertyChanged("Newchapter");
        //    }
        //}


        //public string Website
        //{
        //    get
        //    {
        //        return website;
        //    }
        //    set
        //    {
        //        if (value == website)
        //        {
        //            return;
        //        }
        //        name = value;
        //        NotifyPropertyChanged("Website");
        //    }
        //}


        ///*何处给propertychanged赋值？ : dll封装*/
        //public event PropertyChangedEventHandler PropertyChanged;
        //private void NotifyPropertyChanged(string p)
        //{
        //    if (PropertyChanged != null)
        //    {
        //        PropertyChanged(this, new PropertyChangedEventArgs(p));
        //    }
        //}

    }
}

