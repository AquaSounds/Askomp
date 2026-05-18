/* ------------------------------------------------------------
author: "DSP: UnD3ath, Plugin: Sout"
copyright: "Aqua Sounds"
name: "Askomp"
version: "1.0"
Code generated with Faust 2.85.5 (https://faust.grame.fr)
Compilation options: -a CSharpFaustBase.cs -lang csharp -i -fpga-mem-th 4 -ct 1 -es 1 -mcd 16 -mdd 1024 -mdy 33 -double -ftz 0
------------------------------------------------------------ */
/************************************************************************
    FAUST Architecture File
    Copyright (C) 2021 Mike Oliphant
    ---------------------------------------------------------------------
    This Architecture section is free software; you can redistribute it
    and/or modify it under the terms of the GNU General Public License
    as published by the Free Software Foundation; either version 3 of
    the License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; If not, see <http://www.gnu.org/licenses/>.

    EXCEPTION : As a special exception, you may create a larger work
    that contains this FAUST architecture section and distribute
    that work under terms of your choice, so long as this FAUST
    architecture section is not modified.

 ************************************************************************
 ************************************************************************/

using System;
using System.Collections.Generic;

public class FaustMetaData
{
    Dictionary<string, string> metaData = new Dictionary<string, string>();

    public void Declare(String name, String value)
    {
        metaData[name] = value;
    }

    public string GetValue(string name)
    {
        if (!metaData.ContainsKey(name))
            return null;

        return metaData[name];
    }
}

public class FaustVariableAccessor
{
    public string ID { get; set; }
    public Action<double> SetValue { get; set; }
    public Func<double> GetValue { get; set; }
}

public enum EFaustUIBoxType
{
}

public enum EFaustUIElementType
{
    TabBox,
    HorizontalBox,
    VerticalBox,
    Button,
    CheckBox,
    VerticalSlider,
    HorizontalSlider,
    NumEntry,
    HorizontalBargraph,
    VerticalBargraph
}

public class FaustUIElement
{
    public EFaustUIElementType ElementType { get; set; }
    public string Label { get; set; }
}

public class FaustBoxElement : FaustUIElement
{
    public List<FaustUIElement> Children { get; set; }

    public FaustBoxElement(EFaustUIElementType elementType, string label)
    {
        this.ElementType = elementType;
        this.Label = label;
        this.Children = new List<FaustUIElement>();
    }
}

public class FaustUIVariableElement : FaustUIElement
{
    public FaustVariableAccessor VariableAccessor { get; set; }

    public FaustUIVariableElement(EFaustUIElementType elementType, string label, FaustVariableAccessor variableAccessor)
    {
        this.ElementType = elementType;
        this.Label = label;
        this.VariableAccessor = variableAccessor;
    }
}

public class FaustUIFloatElement : FaustUIVariableElement
{
    public double MinValue { get; set; }
    public double MaxValue { get; set; }

    public FaustUIFloatElement(EFaustUIElementType elementType, string label, FaustVariableAccessor variableAccessor, double minValue, double maxValue)
        : base(elementType, label, variableAccessor)
    {
        this.MinValue = minValue;
        this.MaxValue = maxValue;
    }
}

public class FaustUIWriteableFloatElement : FaustUIFloatElement
{
    public double Step { get; set; }
    public double DefaultValue { get; set; }

    public FaustUIWriteableFloatElement(EFaustUIElementType elementType, string label, FaustVariableAccessor variableAccessor, double defaultValue, double minValue, double maxValue, double step)
        : base(elementType, label, variableAccessor, minValue, maxValue)
    {
        this.DefaultValue = defaultValue;
        this.Step = step;
    }
}

public class FaustUIDefinition
{
    public FaustUIElement RootElement { get; set; }

    Stack<FaustBoxElement> boxStack = new Stack<FaustBoxElement>();

    public void DeclareElementMetaData(string elemendID, string key, string value)
    {
    }

    public void StartBox(FaustBoxElement box)
    {
        if (boxStack.Count == 0)
        {
            RootElement = box;
        }
        else
        {
            boxStack.Peek().Children.Add(box);
        }

        boxStack.Push(box);
    }

    public void EndBox()
    {
        boxStack.Pop();
    }

    public void AddElement(FaustUIElement element)
    {
        boxStack.Peek().Children.Add(element);
    }
}

public interface IFaustDSP
{
    FaustUIDefinition UIDefinition { get; }
    FaustMetaData MetaData { get; }

    int GetNumInputs();
    int GetNumOutputs();
    void ClassInit(int sample_rate);
    void InstanceConstants(int sample_rate);
    void InstanceResetUserInterface();
    void InstanceClear();
    void Init(int sample_rate);
    void InstanceInit(int sample_rate);
    void Compute(int count, double[][] inputs, double[][] outputs);
}

public class dsp
{
    public FaustUIDefinition UIDefinition { get; private set; }
    public FaustMetaData MetaData { get; private set; }

    public dsp()
    {
        UIDefinition = new FaustUIDefinition();
        MetaData = new FaustMetaData();
    }

    public static double FMod(double val1, double val2)
    {
        return val1 % val2;
    }

    public static float FModF(float val1, float val2)
    {
        return val1 % val2;
    }

    public static bool IsInfinity(double d)
    {
        return double.IsNegativeInfinity(d) || double.IsPositiveInfinity(d);
    }

    public static bool IsInfinityF(float d)
    {
        return float.IsNegativeInfinity(d) || float.IsPositiveInfinity(d);
    }
}




public class mydsp : dsp, IFaustDSP
{
	double mydsp_faustpower2_f(double value)
	{
		return value * value;
	}

	
	internal double fVslider0;
	internal double fHslider0;
	int fSampleRate;
	double fConst0;
	internal double fHslider1;
	double[] fRec0 = new double[2];
	internal double fHslider2;
	internal double fVslider1;
	double[] fRec1 = new double[2];
	internal double fHslider3;
	double[] fRec2 = new double[2];
	double[] fRec3 = new double[2];
	
	public mydsp()
	{
		SetMetaData();
		BuildUserInterface();
	}
	
	public int GetNumInputs()
	{
		return 2;
	}

	public int GetNumOutputs()
	{
		return 2;
	}

	
	void SetMetaData()
	{
		MetaData.Declare("author", "DSP: UnD3ath, Plugin: Sout");
		MetaData.Declare("basics.lib/bypass1:author", "Julius Smith");
		MetaData.Declare("basics.lib/name", "Faust Basic Element Library");
		MetaData.Declare("basics.lib/parallelMin:author", "Bart Brouns");
		MetaData.Declare("basics.lib/parallelMin:copyright", "Copyright (c) 2020 Bart Brouns <bart@magnetophon.nl>");
		MetaData.Declare("basics.lib/parallelMin:licence", "GPL-3.0");
		MetaData.Declare("basics.lib/parallelOp:author", "Bart Brouns");
		MetaData.Declare("basics.lib/parallelOp:copyright", "Copyright (c) 2020 Bart Brouns <bart@magnetophon.nl>");
		MetaData.Declare("basics.lib/parallelOp:licence", "GPL-3.0");
		MetaData.Declare("basics.lib/version", "1.22.0");
		MetaData.Declare("compile_options", "-a CSharpFaustBase.cs -lang csharp -i -fpga-mem-th 4 -ct 1 -es 1 -mcd 16 -mdd 1024 -mdy 33 -double -ftz 0");
		MetaData.Declare("compressors.lib/FFcompressor_N_chan:author", "Bart Brouns");
		MetaData.Declare("compressors.lib/FFcompressor_N_chan:license", "GPLv3");
		MetaData.Declare("compressors.lib/name", "Faust Compressor Effect Library");
		MetaData.Declare("compressors.lib/peak_compression_gain_N_chan_db:author", "Bart Brouns");
		MetaData.Declare("compressors.lib/peak_compression_gain_N_chan_db:license", "GPLv3");
		MetaData.Declare("compressors.lib/peak_compression_gain_mono_db:author", "Bart Brouns");
		MetaData.Declare("compressors.lib/peak_compression_gain_mono_db:license", "GPLv3");
		MetaData.Declare("compressors.lib/version", "1.6.0");
		MetaData.Declare("copyright", "Aqua Sounds");
		MetaData.Declare("filename", "askomp_dsp.dsp");
		MetaData.Declare("interpolators.lib/interpolate_linear:author", "Stéphane Letz");
		MetaData.Declare("interpolators.lib/interpolate_linear:licence", "MIT");
		MetaData.Declare("interpolators.lib/name", "Faust Interpolator Library");
		MetaData.Declare("interpolators.lib/version", "1.4.0");
		MetaData.Declare("maths.lib/author", "GRAME");
		MetaData.Declare("maths.lib/copyright", "GRAME");
		MetaData.Declare("maths.lib/license", "LGPL with exception");
		MetaData.Declare("maths.lib/name", "Faust Math Library");
		MetaData.Declare("maths.lib/version", "2.9.0");
		MetaData.Declare("name", "Askomp");
		MetaData.Declare("platform.lib/name", "Generic Platform Library");
		MetaData.Declare("platform.lib/version", "1.3.0");
		MetaData.Declare("routes.lib/name", "Faust Signal Routing Library");
		MetaData.Declare("routes.lib/version", "1.3.0");
		MetaData.Declare("signals.lib/name", "Faust Routing Library");
		MetaData.Declare("signals.lib/onePoleSwitching:author", "Jonatan Liljedahl, revised by Dario Sanfilippo");
		MetaData.Declare("signals.lib/onePoleSwitching:licence", "STK-4.3");
		MetaData.Declare("signals.lib/version", "1.6.0");
		MetaData.Declare("version", "1.0");
	}

	
	void BuildUserInterface()
	{
		UIDefinition.StartBox(new FaustBoxElement(EFaustUIElementType.VerticalBox, "Askomp"));
		UIDefinition.AddElement(new FaustUIWriteableFloatElement(EFaustUIElementType.HorizontalSlider, "Attack", new FaustVariableAccessor {
				ID = "fHslider1",
				SetValue = delegate(double val) { fHslider1 = val; },
				GetValue = delegate { return fHslider1; }
			}
			, 1e+01, 0.0, 5e+01, 0.1));
		UIDefinition.AddElement(new FaustUIWriteableFloatElement(EFaustUIElementType.VerticalSlider, "Negative Threshold", new FaustVariableAccessor {
				ID = "fVslider0",
				SetValue = delegate(double val) { fVslider0 = val; },
				GetValue = delegate { return fVslider0; }
			}
			, 0.0, -36.0, 0.0, 0.1));
		UIDefinition.AddElement(new FaustUIWriteableFloatElement(EFaustUIElementType.VerticalSlider, "Positive Threshold", new FaustVariableAccessor {
				ID = "fVslider1",
				SetValue = delegate(double val) { fVslider1 = val; },
				GetValue = delegate { return fVslider1; }
			}
			, 0.0, -36.0, 0.0, 0.1));
		UIDefinition.AddElement(new FaustUIWriteableFloatElement(EFaustUIElementType.HorizontalSlider, "Raio", new FaustVariableAccessor {
				ID = "fHslider2",
				SetValue = delegate(double val) { fHslider2 = val; },
				GetValue = delegate { return fHslider2; }
			}
			, 2.0, 1.0, 2e+01, 0.1));
		UIDefinition.AddElement(new FaustUIWriteableFloatElement(EFaustUIElementType.HorizontalSlider, "Release", new FaustVariableAccessor {
				ID = "fHslider0",
				SetValue = delegate(double val) { fHslider0 = val; },
				GetValue = delegate { return fHslider0; }
			}
			, 5e+01, 2e+01, 2e+02, 1.0));
		UIDefinition.AddElement(new FaustUIWriteableFloatElement(EFaustUIElementType.HorizontalSlider, "Trim", new FaustVariableAccessor {
				ID = "fHslider3",
				SetValue = delegate(double val) { fHslider3 = val; },
				GetValue = delegate { return fHslider3; }
			}
			, 0.0, -9.0, 9.0, 0.1));
		UIDefinition.EndBox();
	}

	public void ClassInit(int sample_rate)
	{
	}
	
	public void InstanceConstants(int sample_rate)
	{
		fSampleRate = sample_rate;
		fConst0 = 1.0 / Math.Min(1.92e+05, Math.Max(1.0, (double)(fSampleRate)));
	}
	
	public void InstanceResetUserInterface()
	{
		fVslider0 = (double)(0.0);
		fHslider0 = (double)(5e+01);
		fHslider1 = (double)(1e+01);
		fHslider2 = (double)(2.0);
		fVslider1 = (double)(0.0);
		fHslider3 = (double)(0.0);
	}
	
	public void InstanceClear()
	{
		for (int l0 = 0; l0 < 2; l0 = l0 + 1) {
			fRec0[l0] = 0.0;
		}
		for (int l1 = 0; l1 < 2; l1 = l1 + 1) {
			fRec1[l1] = 0.0;
		}
		for (int l2 = 0; l2 < 2; l2 = l2 + 1) {
			fRec2[l2] = 0.0;
		}
		for (int l3 = 0; l3 < 2; l3 = l3 + 1) {
			fRec3[l3] = 0.0;
		}
	}
	
	public void Init(int sample_rate)
	{
		ClassInit(sample_rate);
		InstanceInit(sample_rate);
	}
	
	public void InstanceInit(int sample_rate)
	{
		InstanceConstants(sample_rate);
		InstanceResetUserInterface();
		InstanceClear();
	}
	
	public void Compute(int count, double[][] inputs, double[][] outputs)
	{
		double[] input0 = inputs[0];
		double[] input1 = inputs[1];
		double[] output0 = outputs[0];
		double[] output1 = outputs[1];
		double fSlow0 = (double)(fVslider0);
		double fSlow1 = fSlow0 + 1.5;
		double fSlow2 = 0.001 * (double)(fHslider0);
		int iSlow3 = (Math.Abs(fSlow2) < 2.220446049250313e-16?1:0);
		double fSlow4 = ((iSlow3!=0) ? 0.0 : Math.Exp(-(fConst0 / ((iSlow3!=0) ? 1.0 : fSlow2))));
		double fSlow5 = 0.001 * (double)(fHslider1);
		int iSlow6 = (Math.Abs(fSlow5) < 2.220446049250313e-16?1:0);
		double fSlow7 = ((iSlow6!=0) ? 0.0 : Math.Exp(-(fConst0 / ((iSlow6!=0) ? 1.0 : fSlow5))));
		double fSlow8 = fSlow0 + -1.5;
		double fSlow9 = 0.05 * (1.0 - 1.0 / (double)(fHslider2));
		double fSlow10 = (double)(fVslider1);
		double fSlow11 = fSlow10 + 1.5;
		double fSlow12 = fSlow10 + -1.5;
		double fSlow13 = Math.Pow(1e+01, 0.05 * (double)(fHslider3));
		for (int i0 = 0; i0 < count; i0 = i0 + 1) {
			double fTemp0 = (double)(input0[i0]);
			double fTemp1 = Math.Min(fTemp0, 0.0);
			double fTemp2 = Math.Abs(fTemp1);
			double fTemp3 = ((fTemp2 > fRec0[1]) ? fSlow7 : fSlow4);
			fRec0[0] = fTemp2 * (1.0 - fTemp3) + fRec0[1] * fTemp3;
			double fTemp4 = 2e+01 * Math.Log10(Math.Max(2.2250738585072014e-308, fRec0[0]));
			int iTemp5 = (fTemp4 > fSlow8?1:0) + (fTemp4 > fSlow1?1:0);
			double fTemp6 = Math.Max(fTemp0, 0.0);
			double fTemp7 = Math.Abs(fTemp6);
			double fTemp8 = ((fTemp7 > fRec1[1]) ? fSlow7 : fSlow4);
			fRec1[0] = fTemp7 * (1.0 - fTemp8) + fRec1[1] * fTemp8;
			double fTemp9 = 2e+01 * Math.Log10(Math.Max(2.2250738585072014e-308, fRec1[0]));
			int iTemp10 = (fTemp9 > fSlow12?1:0) + (fTemp9 > fSlow11?1:0);
			output0[i0] = (double)(fSlow13 * (fTemp6 * Math.Pow(1e+01, -(fSlow9 * Math.Max(0.0, ((iTemp10 == 0) ? 0.0 : ((iTemp10 == 1) ? 0.16666666666666666 * mydsp_faustpower2_f(fTemp9 + 1.5 - fSlow10) : fTemp9 - fSlow10))))) + fTemp1 * Math.Pow(1e+01, -(fSlow9 * Math.Max(0.0, ((iTemp5 == 0) ? 0.0 : ((iTemp5 == 1) ? 0.16666666666666666 * mydsp_faustpower2_f(fTemp4 + 1.5 - fSlow0) : fTemp4 - fSlow0)))))));
			double fTemp11 = (double)(input1[i0]);
			double fTemp12 = Math.Min(fTemp11, 0.0);
			double fTemp13 = Math.Abs(fTemp12);
			double fTemp14 = ((fTemp13 > fRec2[1]) ? fSlow7 : fSlow4);
			fRec2[0] = fTemp13 * (1.0 - fTemp14) + fRec2[1] * fTemp14;
			double fTemp15 = 2e+01 * Math.Log10(Math.Max(2.2250738585072014e-308, fRec2[0]));
			int iTemp16 = (fTemp15 > fSlow8?1:0) + (fTemp15 > fSlow1?1:0);
			double fTemp17 = Math.Max(fTemp11, 0.0);
			double fTemp18 = Math.Abs(fTemp17);
			double fTemp19 = ((fTemp18 > fRec3[1]) ? fSlow7 : fSlow4);
			fRec3[0] = fTemp18 * (1.0 - fTemp19) + fRec3[1] * fTemp19;
			double fTemp20 = 2e+01 * Math.Log10(Math.Max(2.2250738585072014e-308, fRec3[0]));
			int iTemp21 = (fTemp20 > fSlow12?1:0) + (fTemp20 > fSlow11?1:0);
			output1[i0] = (double)(fSlow13 * (fTemp17 * Math.Pow(1e+01, -(fSlow9 * Math.Max(0.0, ((iTemp21 == 0) ? 0.0 : ((iTemp21 == 1) ? 0.16666666666666666 * mydsp_faustpower2_f(fTemp20 + 1.5 - fSlow10) : fTemp20 - fSlow10))))) + fTemp12 * Math.Pow(1e+01, -(fSlow9 * Math.Max(0.0, ((iTemp16 == 0) ? 0.0 : ((iTemp16 == 1) ? 0.16666666666666666 * mydsp_faustpower2_f(fTemp15 + 1.5 - fSlow0) : fTemp15 - fSlow0)))))));
			fRec0[1] = fRec0[0];
			fRec1[1] = fRec1[0];
			fRec2[1] = fRec2[0];
			fRec3[1] = fRec3[0];
		}
	}
};

