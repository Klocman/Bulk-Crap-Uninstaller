using System.Linq;
using System.Windows.Forms;

namespace SimpleTreeMapTests
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            treeMap1.ObjectNameGetter = o => o.ToString();
            treeMap1.ObjectValueGetter = o => (int)o;
            treeMap1.Populate(new [] {10,9,8,7,6,5,3,3,3,1}.Cast<object>());
        }
    }
}
