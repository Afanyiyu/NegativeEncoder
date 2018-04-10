﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NegativeEncoder
{
    class Version
    {
        public const string VER = "0.1.0";
        public static string AboutText = "Negative Encoder (消极压制) v" + VER + @"
By Zyzsdy

“消极压制”的含义是什么？
“消极压制”是指最大程度的节约系统资源，加快编码速度的一种压制方式。有时候视频质量并不是首要考虑因素，可以不用“积极的”进行一个精致的压制，何不“消极一把”呢？
本软件调用QuickSync或是NVENC进行硬件加速编码，这是十分有效的一种方式。虽然很多人说生成的质量不如x264的好，但是很多时候也足够用了，不是吗？

使用前需要确保你使用了支持QuickSync的Intel核芯显卡，或是支持NVENC的NVIDIA显卡，才可用本软件编码。

---------------------------------------
本软件集成了各种视频处理工具包，在此向他们致谢：
特别感谢rigaya的QSVEnc和NVEnc，它们是本软件的核心：
* QSVEnc（https://github.com/rigaya/QSVEnc）
* NVEnc (https://github.com/rigaya/NVEnc)
其他工具：
* Avisynth
* Avs2pipmod
* L-SMASH Works
* VSFilterMod
* FFMpeg
* neroAacEnc
* MP4Box
* MKVToolNix
* 本软件的集成方式与界面皆参考了小丸工具箱，小丸也是本软件开发者非常喜欢用的软件，非常感谢小丸工具箱的开发者们为我们带来了这么好用的一款工具
---------------------------------------

高级参数指南：
QuickSync调用QSVEncC.exe，NVENC调用NVEncC.exe
请分别参考原作者的选项指南：

QSVEncC（日文）：http://rigaya34589.blog135.fc2.com/blog-entry-337.html
NVEncC：https://github.com/rigaya/NVEnc/blob/master/NVEncC_Options.en.md

---------------------------------------

更新记录
v0.1.0 (2018-4-10)
* 修复bug
* 更新依赖工具
* 准备开源

v0.0.3 (2018-4-8)
* 修复bug

v0.0.2 (2018-4-8)
* 修复音频压制初始化失败的问题
* 优化UI

v0.0.1 (2018-4-8)
* 初个版本";
    }
}