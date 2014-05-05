﻿namespace ScriptKit.CLR.Html
{
    /// <summary>
    /// CSS Writing Modes Level 3 defines CSS features to support various international script modes, such as left-to-right (e.g., Latin and Indic), right-to-left (e.g., Hebrew and Arabic), bidirectional (e.g., mixed Latin and Arabic) and vertical (e.g., Asian). This article is about the CSS writing-mode property.
    /// </summary>
    [ScriptKit.CLR.Ignore]
    [ScriptKit.CLR.EnumEmit(EnumEmit.StringNameLowerCase)]
    [ScriptKit.CLR.Name("String")]
    public enum WritingMode
    {
        /// <summary>
        /// 
        /// </summary>
        Inherit,

        /// <summary>
        /// Indicates that lines may only break at normal word break points.
        /// </summary>
        [ScriptKit.CLR.Name("horizontal-tb")]
        HorizontalTB, 
        
        /// <summary>
        /// Content flows horizontally from right to left, vertically from top to bottom. The next horizontal line is positioned below the previous line.
        /// </summary>
        [ScriptKit.CLR.Name("rl-tb")]
        RL_TB,

        /// <summary>
        /// Content flows vertically from top to bottom, horizontally from left to right. The next vertical line is positioned to the right of the previous line. For SVG1 documents only, use the deprecated value tb-lr.
        /// </summary>
        [ScriptKit.CLR.Name("vertical-lr")]
        VerticalLR,

        /// <summary>
        /// Content flows vertically from top to bottom, horizontally from right to left. The next vertical line is positioned to the left of the previous line. For SVG1 documents only, use the deprecated value tb or tb-rl.
        /// </summary>
        [ScriptKit.CLR.Name("vertical-rl")]
        VerticalRL,

        /// <summary>
        /// Content flows vertically from bottom to top, horizontally right to left. The next vertical line is positioned to the left of the previous line.
        /// </summary>
        [ScriptKit.CLR.Name("bt-rl")]
        BT_RL,

        /// <summary>
        /// Content flows vertically from bottom to top, horizontally left to right. The next vertical line is positioned to the right of the previous line.
        /// </summary>
        [ScriptKit.CLR.Name("bt-lr")]
        BT_LR,

        /// <summary>
        /// Content flows horizontally from left to right, vertically from bottom to top. The next horizontal line is positioned above the previous line.
        /// </summary>
        [ScriptKit.CLR.Name("lr-bt")]
        LR_BT,

        /// <summary>
        /// Content flows horizontally from right to left, vertically from bottom to top. The next horizontal line is positioned above the previous line.
        /// </summary>
        [ScriptKit.CLR.Name("rl-bt")]
        RL_BT
    }
}
