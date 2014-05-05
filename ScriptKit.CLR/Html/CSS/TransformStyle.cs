﻿namespace ScriptKit.CLR.Html
{
    /// <summary>
    /// The transform-style CSS property determines if the children of the element are positioned in the 3D-space or are flattened in the plane of the element.
    /// </summary>
    [ScriptKit.CLR.Ignore]
    [ScriptKit.CLR.EnumEmit(EnumEmit.StringNameLowerCase)]
    [ScriptKit.CLR.Name("String")]
    public enum TransformStyle
    {
        /// <summary>
        /// 
        /// </summary>
        Inherit,

        /// <summary>
        /// Indicates that the children of the element should be positioned in the 3D-space.
        /// </summary>
        [ScriptKit.CLR.Name("preserve-3d")]
        Preserve3D, 
        
        /// <summary>
        /// Indicates that the children of the element are lying in the plane of the element itself.
        /// </summary>
        Flat
    }
}
