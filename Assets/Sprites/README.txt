ORIGINAL REFERENCE CUTOUTS

These use pixels from the latest wide menu reference, not regenerated artwork. Original perspective, text, icons and relative spacing are retained. Edge transparency is manually traced. Source detail was upscaled; it is not native 12K detail.

Use the five named button PNGs and Support-Rails.png. The rail layer retains the exposed original rail sections; sections concealed by buttons cannot be recovered from the reference. Use color tint for effects; moving or shrinking buttons can expose concealed areas.

UNITY
Import as Sprite, Single, Full Rect, Center Pivot, Input Texture Alpha, Alpha Is Transparency enabled, Compression None.
Use a 3840x2160 reference canvas. Put Support-Rails before the five buttons in sibling order.
All six objects must be direct children of a full-canvas parent, with TOP LEFT anchors (0,1) and CENTER pivot (0.5,0.5), local scale 1.
Enter the values in Unity-Layout-3840x2160.csv. Image Type Simple, Preserve Aspect enabled. Keep Image color and all button tint-state alpha values fully opaque. Support-Rails Raycast Target OFF.
Keep ReferenceOverlay OFF to avoid duplicate buttons.

Assembled-Transparent.png is a visual verification of the complete stack, not a replacement for the five interactive sprites. Reference-Comparison.png includes the source background for comparison.
