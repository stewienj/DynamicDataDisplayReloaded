using DynamicDataDisplay.Common.Auxiliary;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DynamicDataDisplay.Samples.Demos.Custom
{
    /// <summary>
    /// Interaction logic for MouseNaviationSwitchingSample.xaml
    /// </summary>
    public partial class MouseNaviationSwitchingSample : Page
    {
        public MouseNaviationSwitchingSample()
        {
            DataContext = new MouseNaviationSwitchingViewModel();
            InitializeComponent();
            // Grabs the focus so when the user clicks the ListBox it works immediately
            Focus();
        }
    }

    // Below is the view model used in the above class's DataContext

    public class MouseNaviationSwitchingViewModel : D3NotifyPropertyChanged
    {
        // ********************************************************************
        // Private Fields
        // ********************************************************************
        #region Private Fields

        /// <summary>
        /// List of keys for selecting a mouse navigation mode
        /// </summary>
        private List<string> _keys = new[]
        {
            "Default",
            "Freehand Select Area",
            "Rectangle Select Area",
            "Polygon Select Area",
            "Move Selected Area"
        }.ToList(); // Convert to list so we can use IndexOf

        /// <summary>
        /// A set of instructions that match the aboeve keys
        /// </summary>
        private string[] _instructionSet = new[]
        {
            // Default:
            "Current Selection = Default\nDefault mode lets you pan the whole plot by dragging it around with the left mouse button.",
            // Freehand Select Area
            "Current Selection = Freehand Select Area\nFreehand select area lets you hold down the left mouse button and draw a selection area.",
            // Rectangle Select Area
            "Current Selection = Rectangle Select Area\nRectangle select area lets you hold down the left mouse button and drag out rectanglular selection area.",
            // Polygon Select Area
            "Current Selection = Polygon Select Area\nPolygon select area lets you click a series of point to draw a polygon to select an area. To stop hit the ESC key.",
            // Move Selected Area
            "Current Selection = Move Selected Area\nClick and drag anywhere on the plot to move the selected area."
        };

        #endregion Private Fields

        // ********************************************************************
        // Public Methods
        // ********************************************************************


        public MouseNaviationSwitchingViewModel()
        {
            // When the selection area changes signal the polygon on the chart to be redrawn
            SelectionArea.CollectionChanged += (s, e) => RaisePropertyChanged(nameof(PolyLinePoints));
        }

        // ********************************************************************
        // Properties
        // ********************************************************************
        #region Properties

        /// <summary>
        /// A list of available keys for selecting a Mouse Navigation mode
        /// </summary>
        public List<string> Keys => _keys;

        /// <summary>
        /// The currently selected Mouse Navigation mode
        /// </summary>
        private string _selectedMouseMode = "Default";
        public string SelectedMouseMode
        {
            get => _selectedMouseMode;
            set
            {
                SetProperty(ref _selectedMouseMode, value);
                RaisePropertyChanged(nameof(SelectedInstructions));
            }
        }

        /// <summary>
        /// Instructions to display in the instruction box
        /// </summary>
        public string SelectedInstructions
        {
            get
            {
                var index = _keys.IndexOf(SelectedMouseMode);
                if (index < 0 || index >= _instructionSet.Length)
                {
                    return "Unknown Selection Mode";
                }
                else
                {
                    return _instructionSet[index];
                }
            }
        }

        /// <summary>
        /// When is move area mode, indicates whether the user is moving or not moving the selection area
        /// </summary>
        private string _selectionStatus = "Selection Status = Not Selecting";
        public string SelectionStatus
        {
            get => _selectionStatus;
            set
            {
                SetProperty(ref _selectionStatus, value);
            }
        }

        /// <summary>
        /// The selection area that gets written to by the Mouse Navigation handlers
        /// </summary>
        public ObservableCollection<Point> SelectionArea { get; } = new ObservableCollection<Point>();

        #endregion Properties

        /// <summary>
        /// A list of points used to draw a polygon
        /// </summary>
        public IList<Point> PolyLinePoints => SelectionArea.Concat(SelectionArea.Take(1)).ToList();// Close the polygon

        // ********************************************************************
        // Commands
        // ********************************************************************
        #region Commands

        /// <summary>
        /// Command that gets called when the user starts moving the selection
        /// </summary>
        private RelayCommandFactoryD3 _moveStartedCommand = new RelayCommandFactoryD3();
        public ICommand SelectionStartedCommand => _moveStartedCommand.GetCommand(() =>
        {
            SelectionStatus = "Selection Status = Selection In Progress";
        });

        /// <summary>
        /// Command that gets called when the user stops moving the selection
        /// </summary>
        private RelayCommandFactoryD3 _moveStoppedCommand = new RelayCommandFactoryD3();
        public ICommand SelectionStoppedCommand => _moveStoppedCommand.GetCommand(() =>
        {
            SelectionStatus = "Selection Status = Not Selecting";
        });

        #endregion Commands
    }
}
