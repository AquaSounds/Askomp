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

namespace Askomp;

public class AskompDsp : mydsp
{
    public double Attack
    {
        get => fHslider1;
        set => fHslider1 = value;
    }

    public double NegativeThreshold
    {
        get => fVslider0;
        set => fVslider0 = value;
    }

    public double Release
    {
        get => fHslider0;
        set => fHslider0 = value;
    }

    public double Ratio
    {
        get => fHslider2;
        set => fHslider2 = value;
    }

    public double PositiveThreshold
    {
        get => fVslider1;
        set => fVslider1 = value;
    }

    public double Trim
    {
        get => fHslider3;
        set => fHslider3 = value;
    }
}