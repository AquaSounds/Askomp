/*
 * Askomp
 *
 * DSP: UnD3ath, Plugin: UnD3ath
 * 
 * Copyright (C) 2026 UnD3ath / Aqua Sounds
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; version 2 of the License.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Controls;
using Askomp.UI;
using AudioPlugSharp;
using AudioPlugSharpWPF;

namespace Askomp;

public class Askomp : AudioPluginWPF
{
    private static readonly string LogPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
        "vst_error.log");
    
    DoubleAudioIOPort _input;
    DoubleAudioIOPort _output;

    private AudioPluginParameter _posThresholdParameter; //正相位的阈值
    private AudioPluginParameter _negThresholdParameter; //负相位的阈值
    private AudioPluginParameter _attackParameter;       //启动时间
    private AudioPluginParameter _releaseParameter;      //施放时间
    private AudioPluginParameter _ratioParameter;        //压缩比

    private AudioPluginParameter? _linkParameter;         //正负阈值是否Link
    public bool Linked
    {
        get => _linkParameter == null || _linkParameter.ProcessValue > 0.5;
        set { if (_linkParameter != null) _linkParameter.EditValue = value ? 1 : 0; }
    }

    private readonly AskompDsp dsp = new AskompDsp();
    private AskompUi ui;

    private ulong GenerateIntegerId(string seedString)
    {
        var bytes = Encoding.UTF8.GetBytes(seedString);
        SHA256 hashAlg = SHA256.Create();
        byte[] hash = hashAlg.ComputeHash(bytes);
        ulong num = BitConverter.ToUInt64(hash);
        return num;
    }
    
    public Askomp()
    {
        Company = "Aqua Sounds";
        Website = "aqua-sounds.top";
        Contact = "und3ath@outlook.com";
        PluginName = "Askomp";
        PluginCategory = "Fx | Dynamic";
        PluginVersion = "1.0.0";

        PluginID = GenerateIntegerId(Company + PluginName);

        HasUserInterface = true;
        EditorWidth = 500;
        EditorHeight = 400;
        
        ui = new AskompUi(this);
    }

    public override void Initialize()
    {
        base.Initialize();
        dsp.Init(48000);

        InputPorts = [_input = new DoubleAudioIOPort("Input", EAudioChannelConfiguration.Stereo)];
        OutputPorts = [_output = new DoubleAudioIOPort("Output", EAudioChannelConfiguration.Stereo)];

        AddParameter(_posThresholdParameter = new AudioPluginParameter()
        {
            ID = "posThreshold",
            Name = "Positive Threshold",
            MaxValue = 0,
            MinValue = -36,
            DefaultValue = dsp.PositiveThreshold,
            ValueFormat = "{0:0.0}dB"
        });
        
        AddParameter(_negThresholdParameter = new AudioPluginParameter
        {
            ID = "negThreshold",
            Name = "Negative Threshold",
            MaxValue = 0,
            MinValue = -36,
            DefaultValue = dsp.NegativeThreshold,
            ValueFormat = "{0:0.0}dB"
        });

        AddParameter(_attackParameter = new AudioPluginParameter
        {
            ID = "attack",
            Name = "Attack",
            MaxValue = 50,
            MinValue = 0,
            DefaultValue = dsp.Attack,
            ValueFormat = "{0:0.0}ms"
        });
        
        AddParameter(_releaseParameter = new AudioPluginParameter
        {
            ID = "release",
            Name = "Release",
            MaxValue = 200,
            MinValue = 20,
            DefaultValue = dsp.Release,
            ValueFormat = "{0:0.0}ms"
        });

        AddParameter(_ratioParameter = new AudioPluginParameter
        {
            ID = "ratio",
            Name = "Ratio",
            MaxValue = 20,
            MinValue = 1,
            DefaultValue = dsp.Ratio,
            ValueFormat = "{0:0:1}"
        });
        
        AddParameter(_linkParameter = new AudioPluginParameter
        {
            ID = "link",
            Name = "Link",
            MaxValue = 1,
            MinValue = 0,
            DefaultValue = 1,
        });
        
    }

    public override void HandleParameterChange(AudioPluginParameter parameter, double newNormalizedValue, int sampleOffset)
    {
        base.HandleParameterChange(parameter, newNormalizedValue, sampleOffset);
        switch (parameter.ID)
        {
            case "posThreshold":
                _posThresholdParameter.NormalizedEditValue = newNormalizedValue;
                dsp.PositiveThreshold = _posThresholdParameter.ProcessValue;
                ui.Dispatcher.InvokeAsync(() =>
                {
                    ui.SetPosThValue(_posThresholdParameter.ProcessValue, 0);
                });
                break;
            case "negThreshold":
                _negThresholdParameter.NormalizedEditValue = newNormalizedValue;
                dsp.NegativeThreshold = _negThresholdParameter.ProcessValue;
                ui.Dispatcher.InvokeAsync(() =>
                {
                    ui.SetNegThValue(_negThresholdParameter.ProcessValue, 1);
                });
                break;
            case "attack":
                _attackParameter.NormalizedEditValue = newNormalizedValue;
                dsp.Attack = _attackParameter.ProcessValue;
                break;
            case "release":
                _releaseParameter.NormalizedEditValue = newNormalizedValue;
                dsp.Release = _releaseParameter.ProcessValue;
                break;
            case "ratio":
                _ratioParameter.NormalizedEditValue = newNormalizedValue;
                dsp.Ratio = _ratioParameter.ProcessValue;
                ui.Dispatcher.InvokeAsync(() =>
                {
                    ui.SetRatioValue(_ratioParameter.ProcessValue);
                });
                break;
            case "link":
                if (_linkParameter != null)
                {
                    _linkParameter.NormalizedEditValue = newNormalizedValue;
                    ui.Dispatcher.InvokeAsync(() => { ui.ToggleLink(_linkParameter.ProcessValue > 0.5); });
                }
                break;
        }
    }

    public void SetPosTh(double value) { _posThresholdParameter.EditValue = value; }
    
    public void SetNegTh(double value) { _negThresholdParameter.EditValue = value; }
    
    public void SetRatio(double value) { _ratioParameter.EditValue = value; }
    
    public void ResetRatio()
    {
        _ratioParameter.EditValue = _ratioParameter.DefaultValue;
        ui.Dispatcher.InvokeAsync(() => { ui.SetRatioValue(_ratioParameter.ProcessValue); });
    }

    public void SetAttack(double value) { _attackParameter.EditValue = value; }

    public void ResetAttack() 
    { 
        _attackParameter.EditValue = _attackParameter.DefaultValue;
        ui.Dispatcher.InvokeAsync(() => { ui.SetAttackValue(_attackParameter.ProcessValue); });
    }

    public void SetRelease(double value) { _releaseParameter.EditValue = value; }

    public void ResetRelease()
    {
        _releaseParameter.EditValue = _releaseParameter.DefaultValue;
        ui.Dispatcher.InvokeAsync(() => { ui.SetReleaseValue(_releaseParameter.ProcessValue); });
    }
    
    private readonly double _threshold = AudioPluginParameter.DBToLinear(-36);//把渲染波形控制在-36dB以上
    private readonly double[] _waveOut = new double[SampleSize];//渲染波形
    private readonly double[] _waveIn = new double[SampleSize];//渲染波形
    private double _posCompTemp,_negCompTemp;
    private const int SampleSize = 128;     //波形的分辨率
    private int _stepCounter = 0;
    private int _step = 8;     //使用step来降低波形向左步进的速度

    public void SetStep(int value)
    {
        _step = 9 - value;
    }
    
    public override void Process()
    {
        base.Process();
        Host.ProcessAllEvents();
        
        ReadOnlySpan<double> inL = _input.GetAudioBuffer(0);
        ReadOnlySpan<double> inR = _input.GetAudioBuffer(1);
        Span<double> outL = _output.GetAudioBuffer(0);
        Span<double> outR = _output.GetAudioBuffer(1);
        int len = inL.Length;

        double[][] input = [inL.ToArray(), inR.ToArray()];
        double[][] output = new double[2][];
        output[0] = new double[len];
        output[1] = new double[len];
        dsp.Compute(len,input,output);

        double posComp = 0, negComp = 0, oL = 0, oR = 0;
        
        double posCompValue = 0,negCompValue = 0;

        double maxI = 0, maxO = 0;

        _stepCounter++;
        if(_stepCounter > _step)
        {
            //将数组左移
            Array.Copy(_waveIn, 1, _waveIn, 0, SampleSize - 1);
            Array.Copy(_waveOut, 1, _waveOut, 0, SampleSize - 1);
        }

        for (int i = 0; i < len; i++)
        {
            var currentOl = output[0][i];
            var currentOr = output[1][i];
            outL[i] = currentOl;
            outR[i] = currentOr;
            var currentIl = input[0][i];
            var currentIr = input[1][i];
            double mixOut = (currentOl + currentOr) * 0.5;
            double mixIn = (currentIl + currentIr) * 0.5;

            //Delta只是用来缓存压缩的线性值
            var posDelta = double.Abs(double.Max(mixIn,0) - double.Max(mixOut,0));
            var negDelta = double.Abs(double.Min(mixIn, 0) - double.Min(mixOut, 0));
            
            oL = double.Max(oL,output[0][i]);
            oR = double.Max(oR,output[1][i]);
            
            posComp = double.Max(posComp,
                posDelta);
            negComp = double.Max(negComp,
                negDelta);
            
            //分别计算压缩的分贝值
            posCompValue = double.Abs(AudioPluginParameter.LinearToDB(1 - posComp));
            negCompValue = double.Abs(AudioPluginParameter.LinearToDB(1 - negComp));
            
            //判断是否为0来避免除0导致的异常
            maxI = mixIn == 0 ? 0 : double.Max(maxI, double.Abs(mixIn)) * mixIn / double.Abs(mixIn);
            maxO = mixOut == 0 ? 0 : double.Max(maxO, double.Abs(mixOut)) * mixOut / double.Abs(mixOut);
        }

        if (_stepCounter > _step)
        {
            _waveIn[SampleSize - 1] = Math.Abs(maxI) < _threshold ? 0 : maxI;
            _waveOut[SampleSize - 1] = Math.Abs(maxO) < _threshold ? 0 : maxO;
            _stepCounter = 0;
        }

        //Meter Render
        ui.Dispatcher.InvokeAsync(() =>
        {
            // Left Right Meter
            var lMax = ui.LeftOutputMax.ActualWidth -
                       (ui.LeftOutputMeter.Margin.Left + ui.LeftOutputMeter.Margin.Right);
            var rMax = ui.RightOutputMax.ActualWidth -
                       (ui.RightOutputMeter.Margin.Left + ui.RightOutputMeter.Margin.Right);
            ui.LeftOutputMeter.Width =
                double.Lerp(oL, ui.LeftOutputMeter.Width / lMax, 0.95) * lMax;
            ui.RightOutputMeter.Width =
                double.Lerp(oR, ui.RightOutputMeter.Width / rMax, 0.95) * rMax;

            // Positive Negative Meter
            var pMax = ui.PosCompressedMax.ActualHeight -
                       (ui.PosCompressedMeter.Margin.Top + ui.PosCompressedMeter.Margin.Bottom);
            var nMax = ui.NegCompressedMax.ActualHeight -
                       (ui.NegCompressedMeter.Margin.Top + ui.NegCompressedMeter.Margin.Bottom);
            ui.PosCompressedMeter.Height = 
                double.Lerp(posComp, ui.PosCompressedMeter.Height / pMax, 0.95) * pMax;
            ui.NegCompressedMeter.Height =
                double.Lerp(negComp, ui.NegCompressedMeter.Height / nMax, 0.95) * nMax;
            
            // Precise Value of Comp
            _posCompTemp = _posCompTemp > posCompValue ? 
                double.Lerp(posCompValue, _posCompTemp,0.995) : 
                posCompValue;
            _negCompTemp = _negCompTemp > negCompValue ? 
                double.Lerp(negCompValue, _negCompTemp,0.995) : 
                negCompValue;
            
            ui.PosCompValue.Text = _posCompTemp.ToString("0.0");
            ui.NegCompValue.Text = _negCompTemp.ToString("0.0");
            
            ui.DrawWaveOut(_waveOut);
            ui.DrawWaveIn(_waveIn);
            
        });
    }

    public override UserControl GetEditorView()
    {
        return ui;
    }
}