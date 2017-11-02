using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AssettoCorsaSharedMemory;



namespace fuzzy_transmission
{

    public partial class Form1 : Form
    {

        #region Global vars declaration
        public const int locationX = 70;
        public const int locationY = 30;
        public const int locationIncrementX = 50;
        public const int locationIncrementY = 50;


        [DllImport("USER32.DLL")]
        public static extern int SetForegroundWindow(IntPtr hWnd);

        Process p = null;

        Button manualControlButton = new Button();

        Label label_trackbar1 = new Label();
        Label label_trackbar2 = new Label();
        Label label_trackbar3 = new Label();
        Label label_trackbar4 = new Label();
        Label label_trackbar5 = new Label();


        TrackBar trackBar1 = new TrackBar();
        TrackBar trackBar2 = new TrackBar();
        TrackBar trackBar3 = new TrackBar();
        TrackBar trackBar4 = new TrackBar();
        TrackBar trackBar5 = new TrackBar();

        Label labelOutput = new Label();

        ToolTip manualControlButtonTooltip = new ToolTip();


        List<float[]> rules = new List<float[]>();

        #region Fuzzy pertinence functions declaration
        #region Inputs
        //speed
        Dictionary<string, float[]> input1 = new Dictionary<string, float[]>();
        //throttle_pos
        Dictionary<string, float[]> input2 = new Dictionary<string, float[]>();
        //brake
        Dictionary<string, float[]> input3 = new Dictionary<string, float[]>();
        //steering_wheel
        Dictionary<string, float[]> input4 = new Dictionary<string, float[]>();
        //throttle_variation
        Dictionary<string, float[]> input5 = new Dictionary<string, float[]>();

        List<FuzzyPertinenceFunction> listInputPFs = new List<FuzzyPertinenceFunction>();
        List<FuzzyPertinenceFunction> listOutputPFs = new List<FuzzyPertinenceFunction>();

        #endregion

        #region Output
        //gear
        Dictionary<String, float[]> output1 = new Dictionary<string, float[]>();
        #endregion
        #endregion

        #region Fuzzy inputs variable declaration
        float[] inputs = { 0, 0, 0, 0, 0 };
        #endregion

        #region Fuzzy output variables declaration
        //These variables contain the result of the inputs applied to every pertinence function
        #region Inputs
        //speed
        Dictionary<string, float> fuzzy_input1_output = new Dictionary<string, float>();
        //throttle_pos
        Dictionary<string, float> fuzzy_input2_output = new Dictionary<string, float>();
        //brake
        Dictionary<string, float> fuzzy_input3_output = new Dictionary<string, float>();
        //steering_wheel
        Dictionary<string, float> fuzzy_input4_output = new Dictionary<string, float>();
        //throttle_variation
        Dictionary<string, float> fuzzy_input5_output = new Dictionary<string, float>();
        #endregion

        #region Outputs
        //gear
        Dictionary<String, float> fuzzy_output1_output = new Dictionary<string, float>();
        #endregion

        float finalOutput;
        #endregion


        AssettoCorsa ac = new AssettoCorsa();

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

        float speed, throttle_pos, brake, steering_wheel_pos, gear;
        #endregion

        public Form1()
        {
            initializeFormAndObjects();

            #region Fuzzy pertinence functions population
            listInputPFs.Add(new FuzzyPertinenceFunction("speed"));
            listInputPFs.Add(new FuzzyPertinenceFunction("throttle_pos"));
            listInputPFs.Add(new FuzzyPertinenceFunction("brake"));
            listInputPFs.Add(new FuzzyPertinenceFunction("steering_wheel"));
            listInputPFs.Add(new FuzzyPertinenceFunction("throttle_variation"));

            listOutputPFs.Add(new FuzzyPertinenceFunction("gear"));
            //--------------------------------------------------------------------

            List<MembershipFunction> listInputMFs= new List<MembershipFunction>();


            listInputPFs[0].MembershipFunctions = new List<MembershipFunction> { (new MembershipFunction("VL", new float[] { -40.5f, -4.5f, 4.5f, 40.5f })) };
            listInputPFs[0].MembershipFunctions.Add(new MembershipFunction("L", new float[] { 4.5f, 40.5f, 49.5f, 85.5f }));
            listInputPFs[0].MembershipFunctions.Add(new MembershipFunction("M", new float[] { 49.5f, 85.5f, 94.5f, 130.5f }));
            listInputPFs[0].MembershipFunctions.Add(new MembershipFunction("H", new float[] { 94.5f, 130.5f, 139.5f, 175.5f }));
            listInputPFs[0].MembershipFunctions.Add(new MembershipFunction("VH", new float[] { 139.5f, 175.5f, 400f, 500f }));

            listInputPFs[1].MembershipFunctions = new List<MembershipFunction> { (new MembershipFunction("VL", new float[] { -0.225f, -0.025f, 0.025f, 0.225f })) };
            listInputPFs[1].MembershipFunctions.Add(new MembershipFunction("L", new float[] { 0.025f, 0.225f, 0.275f, 0.475f }));
            listInputPFs[1].MembershipFunctions.Add(new MembershipFunction("M", new float[] { 0.275f, 0.475f, 0.525f, 0.725f }));
            listInputPFs[1].MembershipFunctions.Add(new MembershipFunction("H", new float[] { 0.525f, 0.725f, 0.775f, 0.975f }));
            listInputPFs[1].MembershipFunctions.Add(new MembershipFunction("VH", new float[] { 0.775f, 0.975f, 1.025f, 1.225f }));

            listInputPFs[2].MembershipFunctions = new List<MembershipFunction> { (new MembershipFunction("VL", new float[] { -0.225f, -0.025f, 0.025f, 0.225f })) };
            listInputPFs[2].MembershipFunctions.Add(new MembershipFunction("L", new float[] { 0.025f, 0.225f, 0.275f, 0.475f }));
            listInputPFs[2].MembershipFunctions.Add(new MembershipFunction("M", new float[] { 0.275f, 0.475f, 0.525f, 0.725f }));
            listInputPFs[2].MembershipFunctions.Add(new MembershipFunction("H", new float[] { 0.525f, 0.725f, 0.775f, 0.975f }));
            listInputPFs[2].MembershipFunctions.Add(new MembershipFunction("VH", new float[] { 0.775f, 0.975f, 1.025f, 1.225f }));

            listInputPFs[3].MembershipFunctions = new List<MembershipFunction> { (new MembershipFunction("VL", new float[] { -64.8f, -7.2f, 15f, 40 })) };
            listInputPFs[3].MembershipFunctions.Add(new MembershipFunction("L", new float[] { 20f, 35f, 65, 80 }));
            listInputPFs[3].MembershipFunctions.Add(new MembershipFunction("M", new float[] { 65, 85f, 105f, 125 }));
            listInputPFs[3].MembershipFunctions.Add(new MembershipFunction("H", new float[] { 100f, 115f, 145f, 160 }));
            listInputPFs[3].MembershipFunctions.Add(new MembershipFunction("VH", new float[] { 140f, 165f, 187f, 245 }));

            listInputPFs[4].MembershipFunctions = new List<MembershipFunction> { (new MembershipFunction("L", new float[] { -14.4f, -1.6f, 5f, 15 })) };
            listInputPFs[4].MembershipFunctions.Add(new MembershipFunction("M", new float[] { 8f, 18.4f, 21.6f, 32 }));
            listInputPFs[4].MembershipFunctions.Add(new MembershipFunction("H", new float[] { 25f, 35f, 41.6f, 54.4f }));

            //--------------------------------------------------------------------
            listOutputPFs[0].MembershipFunctions = new List<MembershipFunction> { (new MembershipFunction("VL", new float[] { 1, 9f, 13.2f, 14 })) };
            listOutputPFs[0].MembershipFunctions.Add(new MembershipFunction("L", new float[] { 14f, 14.8f, 21.2f, 22 }));
            listOutputPFs[0].MembershipFunctions.Add(new MembershipFunction("M", new float[] { 22f, 22.8f, 37.2f, 38 }));
            listOutputPFs[0].MembershipFunctions.Add(new MembershipFunction("H", new float[] { 38f, 38.8f, 45.2f, 46 }));
            listOutputPFs[0].MembershipFunctions.Add(new MembershipFunction("VH", new float[] { 46f, 46.8f, 50.8f, 59.2f }));
            //--------------------------------------------------------------------

            #endregion

            #region Rules
            //Each number represent the related pertinence function
            //order: inputs, outputs, rule weight
            //order: speed, throttle_pos, brake, steering_wheel, throttle_variation, GEAR(output), RULE WEIGHT(from 0 to 1)
            rules.Add(new float[] { 4, -1, -1, 0, 2, 5, 1 });
            rules.Add(new float[] { 4, -1, -1, 4, 2, 5, 1 });
            rules.Add(new float[] { 3, -1, -1, 0, 2, 4, 1 });
            rules.Add(new float[] { 3, -1, -1, 4, 2, 4, 1 });
            rules.Add(new float[] { 4, -1, -1, 1, 2, 5, 1 });
            rules.Add(new float[] { 4, -1, -1, 3, 2, 5, 1 });
            rules.Add(new float[] { 3, -1, -1, 1, 2, 4, 1 });
            rules.Add(new float[] { 3, -1, -1, 3, 2, 4, 1 });
            rules.Add(new float[] { 2, -1, -1, 0, 2, 4, 1 });
            rules.Add(new float[] { 2, -1, -1, 4, 2, 4, 1 });
            rules.Add(new float[] { 1, -1, -1, 0, 2, 3, 1 });
            rules.Add(new float[] { 1, -1, -1, 4, 2, 3, 1 });
            rules.Add(new float[] { 2, -1, -1, 1, 2, 4, 1 });
            rules.Add(new float[] { 2, -1, -1, 3, 2, 4, 1 });
            rules.Add(new float[] { 1, -1, -1, 1, 2, 3, 1 });
            rules.Add(new float[] { 1, -1, -1, 3, 2, 3, 1 });
            rules.Add(new float[] { 4, -1, -1, 0, 1, 5, 1 });
            rules.Add(new float[] { 4, -1, -1, 4, 1, 5, 1 });
            rules.Add(new float[] { 3, -1, -1, 0, 1, 4, 1 });
            rules.Add(new float[] { 3, -1, -1, 4, 1, 4, 1 });
            rules.Add(new float[] { 4, -1, -1, 1, 1, 5, 1 });
            rules.Add(new float[] { 4, -1, -1, 3, 1, 5, 1 });
            rules.Add(new float[] { 3, -1, -1, 1, 1, 4, 1 });
            rules.Add(new float[] { 3, -1, -1, 3, 1, 4, 1 });
            rules.Add(new float[] { 2, -1, -1, 0, 1, 4, 1 });
            rules.Add(new float[] { 2, -1, -1, 4, 1, 4, 1 });
            rules.Add(new float[] { 1, -1, -1, 0, 1, 3, 1 });
            rules.Add(new float[] { 1, -1, -1, 4, 1, 3, 1 });
            rules.Add(new float[] { 2, -1, -1, 1, 1, 4, 1 });
            rules.Add(new float[] { 2, -1, -1, 3, 1, 4, 1 });
            rules.Add(new float[] { 1, -1, -1, 1, 1, 3, 1 });
            rules.Add(new float[] { 1, -1, -1, 3, 1, 3, 1 });
            rules.Add(new float[] { 4, 4, 0, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 4, 0, -1, 0, 3, 1 });
            rules.Add(new float[] { 2, 4, 0, -1, 0, 2, 1 });
            rules.Add(new float[] { 1, 4, 0, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 4, 0, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 4, 1, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 4, 1, -1, 0, 3, 1 });
            rules.Add(new float[] { 2, 4, 1, -1, 0, 2, 1 });
            rules.Add(new float[] { 1, 4, 1, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 4, 1, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 4, 2, -1, 0, 4, 1 });
            rules.Add(new float[] { 3, 4, 2, -1, 0, 3, 1 });
            rules.Add(new float[] { 2, 4, 2, -1, 0, 2, 1 });
            rules.Add(new float[] { 1, 4, 2, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 4, 2, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 4, 3, -1, 0, 4, 1 });
            rules.Add(new float[] { 3, 4, 3, -1, 0, 3, 1 });
            rules.Add(new float[] { 2, 4, 3, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 4, 3, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 4, 3, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 4, 4, -1, 0, 4, 1 });
            rules.Add(new float[] { 3, 4, 4, -1, 0, 3, 1 });
            rules.Add(new float[] { 2, 4, 4, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 4, 4, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 4, 4, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 3, 0, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 3, 0, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 3, 0, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 3, 0, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 3, 0, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 3, 1, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 3, 1, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 3, 1, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 3, 1, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 3, 1, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 3, 2, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 3, 2, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 3, 2, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 3, 2, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 3, 2, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 3, 3, -1, 0, 4, 1 });
            rules.Add(new float[] { 3, 3, 3, -1, 0, 3, 1 });
            rules.Add(new float[] { 2, 3, 3, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 3, 3, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 3, 3, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 3, 4, -1, 0, 4, 1 });
            rules.Add(new float[] { 3, 3, 4, -1, 0, 3, 1 });
            rules.Add(new float[] { 2, 3, 4, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 3, 4, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 3, 4, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 2, 0, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 2, 0, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 2, 0, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 2, 0, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 2, 0, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 2, 1, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 2, 1, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 2, 1, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 2, 1, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 2, 1, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 2, 2, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 2, 2, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 2, 2, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 2, 2, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 2, 2, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 2, 3, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 2, 3, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 2, 3, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 2, 3, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 2, 3, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 2, 4, -1, 0, 4, 1 });
            rules.Add(new float[] { 3, 2, 4, -1, 0, 3, 1 });
            rules.Add(new float[] { 2, 2, 4, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 2, 4, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 2, 4, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 1, 0, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 1, 0, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 1, 0, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 1, 0, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 1, 0, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 1, 1, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 1, 1, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 1, 1, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 1, 1, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 1, 1, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 1, 2, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 1, 2, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 1, 2, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 1, 2, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 1, 2, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 1, 3, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 1, 3, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 1, 3, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 1, 3, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 1, 3, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 1, 4, -1, 0, 4, 1 });
            rules.Add(new float[] { 3, 1, 4, -1, 0, 3, 1 });
            rules.Add(new float[] { 2, 1, 4, -1, 0, 2, 1 });
            rules.Add(new float[] { 1, 1, 4, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 1, 4, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 0, 0, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 0, 0, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 0, 0, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 0, 0, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 0, 0, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 0, 1, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 0, 1, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 0, 1, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 0, 1, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 0, 1, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 0, 2, -1, 0, 5, 1 });
            rules.Add(new float[] { 3, 0, 2, -1, 0, 4, 1 });
            rules.Add(new float[] { 2, 0, 2, -1, 0, 3, 1 });
            rules.Add(new float[] { 1, 0, 2, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 0, 2, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 0, 3, -1, 0, 4, 1 });
            rules.Add(new float[] { 3, 0, 3, -1, 0, 3, 1 });
            rules.Add(new float[] { 2, 0, 3, -1, 0, 2, 1 });
            rules.Add(new float[] { 1, 0, 3, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 0, 3, -1, 0, 1, 1 });
            rules.Add(new float[] { 4, 0, 4, -1, 0, 4, 1 });
            rules.Add(new float[] { 3, 0, 4, -1, 0, 3, 1 });
            rules.Add(new float[] { 2, 0, 4, -1, 0, 2, 1 });
            rules.Add(new float[] { 1, 0, 4, -1, 0, 2, 1 });
            rules.Add(new float[] { 0, 0, 4, -1, 0, 1, 1 });
            #endregion
        }

        private void initializeFormAndObjects()
        {
            InitializeComponent();

            //
            // Assetto Corsa process and timer
            //
            ac = new AssettoCorsa();

            timer.Interval = 100;
            timer.Tick += new EventHandler(setTrackbarValue);
            timer.Stop(); //start in manual control
            //timer.Start();


            // 
            // Buttons
            // 
            manualControlButtonTooltip.SetToolTip(manualControlButton, "Click to connect to Assetto Corsa");
            manualControlButton.Location = new Point(locationX + 70, locationY);
            manualControlButton.Click += new EventHandler(this.manualControlButtonClicked);
            manualControlButton.Size = new System.Drawing.Size(64, 64);
            manualControlButton.BackgroundImage = fuzzy_transmission.Properties.Resources.manual_input;
            manualControlButton.BackgroundImageLayout = ImageLayout.Stretch;

            //
            //Trackbars
            //
            trackBar1.Name = "Speed";
            trackBar2.Name = "Throttle position";
            trackBar3.Name = "Brake";
            trackBar4.Name = "Steering Wheel";
            trackBar5.Name = "Throttle variation";

            trackBar1.Location = new Point(locationX, locationY + locationIncrementY * 2);
            trackBar2.Location = new Point(locationX, locationY + locationIncrementY * 3);
            trackBar3.Location = new Point(locationX, locationY + locationIncrementY * 4);
            trackBar4.Location = new Point(locationX, locationY + locationIncrementY * 5);
            trackBar5.Location = new Point(locationX, locationY + locationIncrementY * 6);

            trackBar1.SetRange(0, 400);
            trackBar2.SetRange(0, 100);
            trackBar3.SetRange(0, 100);
            trackBar4.SetRange(0, 180);
            trackBar5.SetRange(0, 40);

            trackBar1.ValueChanged += new System.EventHandler(this.label_trackbar1_TextChanged);
            trackBar2.ValueChanged += new System.EventHandler(this.label_trackbar2_TextChanged);
            trackBar3.ValueChanged += new System.EventHandler(this.label_trackbar3_TextChanged);
            trackBar4.ValueChanged += new System.EventHandler(this.label_trackbar4_TextChanged);
            trackBar5.ValueChanged += new System.EventHandler(this.label_trackbar5_TextChanged);

            //
            //Labels
            //
            label_trackbar1.Location = new Point(locationX + locationIncrementX * 2, trackBar1.Location.Y);
            label_trackbar2.Location = new Point(locationX + locationIncrementX * 2, trackBar2.Location.Y);
            label_trackbar3.Location = new Point(locationX + locationIncrementX * 2, trackBar3.Location.Y);
            label_trackbar4.Location = new Point(locationX + locationIncrementX * 2, trackBar4.Location.Y);
            label_trackbar5.Location = new Point(locationX + locationIncrementX * 2, trackBar5.Location.Y);

            label_trackbar1.Text = "Speed";
            label_trackbar2.Text = "Throttle position";
            label_trackbar3.Text = "Brake";
            label_trackbar4.Text = "Steering Wheel";
            label_trackbar5.Text = "Throttle variation";

            label_trackbar1.Width = 300;
            label_trackbar2.Width = 300;
            label_trackbar3.Width = 300;
            label_trackbar4.Width = 300;
            label_trackbar5.Width = 300;

            labelOutput.Location = new Point(locationX, locationY + locationIncrementY * 7);
            labelOutput.Text = "Output: ";

            //
            //Form1
            //
            this.Text = "Fuzzy automatic transmission controller";
            this.Controls.Add(trackBar1);
            this.Controls.Add(trackBar2);
            this.Controls.Add(trackBar3);
            this.Controls.Add(trackBar4);
            this.Controls.Add(trackBar5);
            this.Controls.Add(label_trackbar1);
            this.Controls.Add(label_trackbar2);
            this.Controls.Add(label_trackbar3);
            this.Controls.Add(label_trackbar4);
            this.Controls.Add(label_trackbar5);
            this.Controls.Add(manualControlButton);
            this.Controls.Add(labelOutput);

            this.MaximumSize = this.MinimumSize = this.Size;
        }

        private void manualControlButtonClicked(object sender, EventArgs e)
        {
            if(timer.Enabled) {
                //Switch to manual control
                timer.Stop();
                trackBar1.ValueChanged += new System.EventHandler(this.label_trackbar1_TextChanged);
                trackBar2.ValueChanged += new System.EventHandler(this.label_trackbar2_TextChanged);
                trackBar3.ValueChanged += new System.EventHandler(this.label_trackbar3_TextChanged);
                trackBar4.ValueChanged += new System.EventHandler(this.label_trackbar4_TextChanged);
                trackBar5.ValueChanged += new System.EventHandler(this.label_trackbar5_TextChanged);

                trackBar1.Enabled = true;
                trackBar2.Enabled = true;
                trackBar3.Enabled = true;
                trackBar4.Enabled = true;
                trackBar5.Enabled = true;

                manualControlButtonTooltip.SetToolTip(manualControlButton, "Click to connect to Assetto Corsa");
                manualControlButton.BackgroundImage = fuzzy_transmission.Properties.Resources.manual_input;

            }
            else {
                //Switch to assetto corsa connection
                timer.Start();
                trackBar1.ValueChanged -= new System.EventHandler(this.label_trackbar1_TextChanged);
                trackBar2.ValueChanged -= new System.EventHandler(this.label_trackbar2_TextChanged);
                trackBar3.ValueChanged -= new System.EventHandler(this.label_trackbar3_TextChanged);
                trackBar4.ValueChanged -= new System.EventHandler(this.label_trackbar4_TextChanged);
                trackBar5.ValueChanged -= new System.EventHandler(this.label_trackbar5_TextChanged);

                trackBar1.Enabled = false;
                trackBar2.Enabled = false;
                trackBar3.Enabled = false;
                trackBar4.Enabled = false;
                trackBar5.Enabled = false;

                manualControlButtonTooltip.SetToolTip(manualControlButton, "Click to change to manual control inputs");
                manualControlButton.BackgroundImage = fuzzy_transmission.Properties.Resources.assetto_icon;

                initializeAssettoConnection();
            }
        }

        private void initializeAssettoConnection()
        {
            p = Process.GetProcessesByName("acs").FirstOrDefault();
            //SetForegroundWindow(p.MainWindowHandle);

            if(ac.IsRunning) {
                ac.Stop();
                if(!ac.IsRunning)
                    MessageBox.Show("Assetto Corsa connection failed to stop");
                else {
                    ac.StaticInfoUpdated -= ac_StaticInfoUpdated;
                    ac.PhysicsUpdated -= ac_PhysicsUpdated;
                    MessageBox.Show("Assetto Corsa connection stopped successfully");
                }
            }
            else {
                //ac.StaticInfoInterval = 5000; // Get StaticInfo updates ever 5 seconds
                //ac.StaticInfoUpdated += ac_StaticInfoUpdated; // Add event listener for StaticInfo

                ac.PhysicsInterval = 100;
                ac.PhysicsUpdated += ac_PhysicsUpdated;

                ac.Start(); // Connect to shared memory and start interval timers 
                ac.Start();

                //Console.ReadKey();

                if(ac.IsRunning)
                    MessageBox.Show("Assetto Corsa connection was successful!");
                else
                    MessageBox.Show("Assetto Corsa connection failed");
            }
        }

        private void calculateOutput() {
            
            FuzzyPertinenceFunction currentPF;
            currentPF = listInputPFs[0];
            calculateMFoutput(currentPF, trackBar1.Value);
            currentPF = listInputPFs[1];
            calculateMFoutput(currentPF, trackBar2.Value / 100f);
            currentPF = listInputPFs[2];
            calculateMFoutput(currentPF, trackBar3.Value / 100f);
            currentPF = listInputPFs[3];
            calculateMFoutput(currentPF, trackBar4.Value);
            currentPF = listInputPFs[4];
            calculateMFoutput(currentPF, trackBar5.Value);


            finalOutput = applyRulesToOutput();

            //MessageBox.Show(finalOutput.ToString());
            labelOutput.Text = "Output: " + finalOutput.ToString();
        }

        private void calculateOutputDirect(float speed, float throttlePos, float breakPos, float steeringWheelAngle, float throttleVariation) {
            
            FuzzyPertinenceFunction currentPF;
            currentPF = listInputPFs[0];
            calculateMFoutput(currentPF, speed);
            currentPF = listInputPFs[1];
            calculateMFoutput(currentPF, throttlePos);
            currentPF = listInputPFs[2];
            calculateMFoutput(currentPF, breakPos);
            currentPF = listInputPFs[3];
            calculateMFoutput(currentPF, steeringWheelAngle);
            currentPF = listInputPFs[4];
            calculateMFoutput(currentPF, throttleVariation);

            finalOutput = applyRulesToOutput();

            finalOutput = (float)Math.Floor((double)(Math.Max(1, finalOutput/10)));

            applyOutputToAssetto();
        }

        private void applyOutputToAssetto()
        {
            if(p != null) {
                IntPtr h = p.MainWindowHandle;
                SetForegroundWindow(h);

                int howManyGearsToChange = (int) Math.Abs((int) finalOutput - gear);

                Console.WriteLine("  Gear:   " + gear + " Calculated output: " + finalOutput + " How many gears to change: " + howManyGearsToChange + "\n2");

                for(int i = 0; i < howManyGearsToChange; i++) {
                    if(finalOutput < gear)
                        SendKeys.SendWait("2");
                    else
                        SendKeys.SendWait("1");
                }
            }
        }

        private void calculateMFoutput(FuzzyPertinenceFunction currentPF, float input)
        {
            for(int i = 0; i < currentPF.MembershipFunctions.Count; i++) {
                currentPF.MembershipFunctions[i].MFoutput = applyInputToTrapezoid(input, currentPF.MembershipFunctions[i].MFpoints);
            }
        }

        static float applyInputToTrapezoid(float x, float[] parameters)
        {
            float a = parameters[0], b = parameters[1], c = parameters[2], d = parameters[3];
            float y1 = 0, y2 = 0;

            if(a > b) {
                //System.Console.Out.WriteLine("Illegal parameters in applyInputToTrapezoid() --> a > b");
            }
            else if(c > d) {
                //System.Console.Out.WriteLine("Illegal parameters in applyInputToTrapezoid() --> c > d");
            }

            /*               
             *              / ------------ \
             *             / |            | \
             *            /  |            |  \
             *        ___/___|____________|___\___
             *          a    b            c   d
             */
            if((x < a) || (x > d)) { /* Outside of the support of a fuzzy set */
                //System.Console.Out.WriteLine("Probable wrong parameters in applyInputToTrapezoid() --> x < a or x > d");
                return 0.0f;
            }

            /* The rest is combination of two signals:
             * First signal:    
             *               
             *          / ------------ 
             *         / |            
             *        /  |            
             *       /___|____________
             *       a   b            
             */
            if(x < b)/* x >= a && x < b */
                y1 = (x - a) / (b - a);
            else /* x >= b */
                y1 = 1;

            /* Second signal:    
             *               
             *       ------------ \
             *                   | \
             *                   |  \
             *       ____________|___\
             *                   c   d
             */
            if(x > c) /* x > c && x <= d */
                y2 = (d - x) / (d - c);
            else /* x <= c */
                y2 = 1;

            return (Math.Min(y1, y2));
        }

        private float applyRulesToOutput()
        {
            //For each rule must find the minimum pertinence between the inputs involved 
            //and calculate the output membership function pertinence
            float[] currentRule;
            int numberOfOutputs = listOutputPFs.Count;
            FuzzyPertinenceFunction currentPF;
            int i, j;
            float minimumPertinence;
            
            //List to hold the outcome of applying the inputs to the rules
            List<MembershipFunction> listMFsAfterInputs = new List<MembershipFunction>();

            for(i = 0; i < rules.Count; i++) {
                currentRule = rules[i];

                minimumPertinence = 1;
                //The rules are composed of inputs + outputs + rule weight
                //therefore to access only the inputs is necessary to
                for(j = 0; j < currentRule.Length - numberOfOutputs - 1; j++) {
                    currentPF = listInputPFs[j];
                    if(currentRule[j] > -1)
                        minimumPertinence = Math.Min(minimumPertinence, currentPF.MembershipFunctions[(int)currentRule[j]].MFoutput);     
                }


                listMFsAfterInputs.Add(new MembershipFunction(listOutputPFs[0].getMFslinguisticVariable(((int) currentRule[5])-1), listOutputPFs[0].MembershipFunctions[((int) currentRule[5]) - 1].MFpoints, minimumPertinence));
            }

            return defuzzyfication(listMFsAfterInputs);
        }

        private float defuzzyfication(List<MembershipFunction> listMFsAfterInputs)
        {
            /*
            int intervals = (int)((listOutputPFs[0].MembershipFunctions[listOutputPFs[0].MembershipFunctions.Count - 1].MFpoints.Max()
                                    - listOutputPFs[0].MembershipFunctions[listOutputPFs[0].MembershipFunctions.Count - 1].MFpoints.Min())
                                    / numberDefuzzPoints);

            float[] outputVector = new float[(int)numberDefuzzPoints];

            foreach(MembershipFunction MF in listMFsAfterInputs) {
                for(int i = 0; i < intervals; i++) {
                    outputVector[intervals * i] = Math.Max(MF.MFpoints[intervals], outputVector[intervals * i]);
                }
            }
            */
            float numerator = 0;
            float denominator = 0;

            foreach(MembershipFunction membershipFunction in listMFsAfterInputs) {
                numerator += membershipFunction.Centorid() * membershipFunction.Area();
                denominator += membershipFunction.Area();
            }

            return numerator / denominator;
        }

        private void ac_StaticInfoUpdated(object sender, StaticInfoEventArgs e)
        {
            // Print out some data from StaticInfo
            /*
            Console.WriteLine("StaticInfo");
            Console.WriteLine("  Car Model: " + e.StaticInfo.CarModel);
            Console.WriteLine("  Track:     " + e.StaticInfo.Track);
            Console.WriteLine("  Max RPM:   " + e.StaticInfo.MaxRpm);
            */
        }

        private void ac_PhysicsUpdated(object sender, PhysicsEventArgs e)
        {
            // Print out some data from StaticInfo
            /*
            Console.WriteLine("  PhysicsInfo");
            Console.WriteLine("  Speed: " + e.Physics.SpeedKmh);
            Console.WriteLine("  Brake:     " + e.Physics.Brake); //0 to 1
            Console.WriteLine("  SteerAngle:   " + e.Physics.SteerAngle); //-1 to 1
            Console.WriteLine("  Gear:   " + e.Physics.Gear);
            Console.WriteLine("  Gas:   " + e.Physics.Gas); //0 to 1
            */

            calculateOutputDirect(e.Physics.SpeedKmh, e.Physics.Gas, e.Physics.Brake, 90f*(1f + (e.Physics.SteerAngle)), 0);

            /*
            SetTrackbarValue(trackBar1, (int) e.Physics.SpeedKmh);
            SetTrackbarValue(trackBar2, (int) e.Physics.Gas*100);
            SetTrackbarValue(trackBar3, (int) e.Physics.Brake*100);
            SetTrackbarValue(trackBar4, (int) (90f * (1f + (e.Physics.SteerAngle))));
            */

            speed = e.Physics.SpeedKmh;
            throttle_pos = e.Physics.Gas * 100f;
            brake = e.Physics.Brake*100f;
            steering_wheel_pos = e.Physics.SteerAngle*100f;
            gear = e.Physics.Gear;
        }

        #region Trackbars events
        public void setTrackbarValue(object sender, System.EventArgs e)
        {
            trackBar1.Value = (int) speed;
            trackBar2.Value = (int) throttle_pos;
            trackBar3.Value = (int) brake;
            trackBar4.Value = (int) ((Math.Abs(steering_wheel_pos)));

            label_trackbar1.Text = "Speed " + speed.ToString();
            label_trackbar2.Text = "Throttle position " + throttle_pos.ToString();
            label_trackbar3.Text = "Brake " + brake.ToString();
            label_trackbar4.Text = "Steering Wheel " + steering_wheel_pos.ToString();

            labelOutput.Text = "Output: " + finalOutput;
        }


        void label_trackbar1_TextChanged(Object sender, EventArgs e)
        {
            label_trackbar1.Text = "Speed" + trackBar1.Value.ToString();
            calculateOutput();
        }

        void label_trackbar2_TextChanged(Object sender, EventArgs e)
        {
            label_trackbar2.Text = "Throttle position" + trackBar2.Value.ToString();
            calculateOutput();
        }

        void label_trackbar3_TextChanged(Object sender, EventArgs e)
        {
            label_trackbar3.Text = "Brake" + trackBar3.Value.ToString();
            calculateOutput();
        }

        void label_trackbar4_TextChanged(Object sender, EventArgs e)
        {
            label_trackbar4.Text = "Steering Wheel" + trackBar4.Value.ToString();
            calculateOutput();
        }

        void label_trackbar5_TextChanged(Object sender, EventArgs e)
        {
            label_trackbar5.Text = "Throttle variation" + trackBar5.Value.ToString();
            calculateOutput();
        }
        #endregion

    }
}
