/*
* ALChColor is based on Björn Ottosson OkLab
* Implemented in C# by SamsamTS
* https://bottosson.github.io/posts/oklab/
* Copyright (c) 2020 Björn Ottosson
* Permission is hereby granted, free of charge, to any person obtaining a copy of
* this software and associated documentation files (the "Software"), to deal in
* the Software without restriction, including without limitation the rights to
* use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
* of the Software, and to permit persons to whom the Software is furnished to do
* so, subject to the following conditions:
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/

using System;
using System.Windows.Media;

namespace Memoria.Launcher
{
    public struct ALChColor
    {
        /// <summary>
        /// Alpha, expressed as a linear value [0-1]
        /// </summary>
        public Single A { readonly get => _A; set => _A = Math.Min(Math.Max(value, 0f), 1f); }
        /// <summary>
        /// Lightness, expressed as a perceptual value [0-1]
        /// </summary>
        public Single L { readonly get => _L; set => _L = value; }
        /// <summary>
        /// Chroma, expressed as a perceptual value [0-1]
        /// </summary>
        public Single C { readonly get => _C; set => _C = value; }
#pragma warning disable IDE1006 // Naming Styles
        /// <summary>
        /// Hue, expressed as an angle in radians [0-2π[.
        /// </summary>
        public Single h
        {
            readonly get => _h;
            set
            {
                while (value < 0f) value += (float)Math.PI * 2f;
                while (value >= (float)Math.PI * 2f) value -= (float)Math.PI * 2f;
                _h = value;
            }
        }
#pragma warning restore IDE1006 // Naming Styles
        /// <summary>
        /// Hue, expressed as angle in degrees [0-360[
        /// </summary>
        public Single H
        {
            readonly get => 180f * h / (float)Math.PI;
            set => h = value * (float)Math.PI / 180f;
        }

        /// <summary>
        /// Creates a new <see cref="ALChColor"/> instance.
        /// </summary>
        public ALChColor()
        {
            _A = 1f;
            _L = 0f;
            _C = 0f;
            _h = 0f;
        }

        public ALChColor(Single A, Single L, Single C, Single h)
        {
            this.A = A;
            this.L = L;
            this.C = C;
            this.h = h;
        }

        public ALChColor(ALChColor c)
        {
            _A = c._A;
            _L = c._L;
            _C = c._C;
            _h = c._h;
        }

        public ALChColor(Color c)
        {
            Single r = GammaToLinear(c.ScR);
            Single g = GammaToLinear(c.ScG);
            Single b = GammaToLinear(c.ScB);
            A = GammaToLinear(c.ScA);

            Single l_ = (float)Math.Pow(0.4122214708f * r + 0.5363325363f * g + 0.0514459929f * b, 1f / 3f);
            Single m_ = (float)Math.Pow(0.2119034982f * r + 0.6806995451f * g + 0.1073969566f * b, 1f / 3f);
            Single s_ = (float)Math.Pow(0.0883024619f * r + 0.2817188376f * g + 0.6299787005f * b, 1f / 3f);

            L = 0.2104542553f * l_ + 0.7936177850f * m_ - 0.0040720468f * s_;
            Single a_ = 1.9779984951f * l_ - 2.4285922050f * m_ + 0.4505937099f * s_;
            Single b_ = 0.0259040371f * l_ + 0.7827717662f * m_ - 0.8086757660f * s_;

            C = (float)Math.Sqrt(a_ * a_ + b_ * b_);
            h = (float)Math.Atan2(b_, a_);
        }

        /// <summary>
        /// Blends specified color into the current color by a factor amount
        /// </summary>
        /// <param name="color">The <b>color</b> to blend in.</param>
        /// <param name="factor">The <b>factor</b> to blend by, where 0 is none of the blend color and 1 is all of it.</param>
        /// <returns>Returns the current color after blending</returns>
        public ALChColor Blend(ALChColor color, float factor)
        {
            if (factor <= 0f) return this;
            if (factor >= 1f)
            {
                _A = color._A;
                _L = color._L;
                _C = color._C;
                _h = color._h;
                return this;
            }

            Single a1 = _C * (float)Math.Cos(_h);
            Single b1 = _C * (float)Math.Sin(_h);

            Single a2 = color._C * (float)Math.Cos(color._h);
            Single b2 = color._C * (float)Math.Sin(color._h);

            _A = Lerp(_A, color._A, factor);
            _L = Lerp(_L, color._L, factor);

            a1 = Lerp(a1, a2, factor);
            b1 = Lerp(b1, b2, factor);
            C = (float)Math.Sqrt(a1 * a1 + b1 * b1);
            h = (float)Math.Atan2(b1, a1);

            return this;
        }

        private Single Lerp(Single v0, Single v1, Single t)
        {
            return (1 - t) * v0 + t * v1;
        }

        public static implicit operator ALChColor(Color color)
        {
            return new ALChColor(color);
        }

        public static implicit operator Color(ALChColor c)
        {
            Single a_ = c._C * (float)Math.Cos(c._h);
            Single b_ = c._C * (float)Math.Sin(c._h);

            Single l_ = c._L + 0.3963377774f * a_ + 0.2158037573f * b_;
            Single m_ = c._L - 0.1055613458f * a_ - 0.0638541728f * b_;
            Single s_ = c._L - 0.0894841775f * a_ - 1.2914855480f * b_;

            Single l = l_ * l_ * l_;
            Single m = m_ * m_ * m_;
            Single s = s_ * s_ * s_;

            Single r = 4.0767416621f * l - 3.3077115913f * m + 0.2309699292f * s;
            Single g = -1.2684380046f * l + 2.6097574011f * m - 0.3413193965f * s;
            Single b = -0.0041960863f * l - 0.7034186147f * m + 1.7076147010f * s;

            return Color.FromScRgb(
                LinearToGamma(c._A),
                LinearToGamma(Math.Min(Math.Max(r, 0f), 1f)),
                LinearToGamma(Math.Min(Math.Max(g, 0f), 1f)),
                LinearToGamma(Math.Min(Math.Max(b, 0f), 1f)));
        }

        private static Single GammaToLinear(Single c)
        {
            return c; // System.Windows.Media.Color is already linear
            //return c >= 0.04045f ? MathF.Pow((c + 0.055f) / 1.055f, 2.4f) : c / 12.92f;
        }
        private static Single LinearToGamma(Single c)
        {
            return c; // System.Windows.Media.Color is already linear
            //return c >= 0.0031308f ? 1.055f * MathF.Pow(c, 1f / 2.4f) - 0.055f : 12.92f * c;
        }

        private Single _A;
        private Single _C;
        private Single _L;
        private Single _h;
    }
}
