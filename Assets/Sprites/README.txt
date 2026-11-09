TIDY UP - APPROVED PAUSE MENU SKIN

Four transparent PNG assets:
PausePanel.png - gold-edged walnut panel, divider and recessed timer area. No text or buttons baked in.
PrimaryButton.png - blank teal Resume button.
SecondaryButtonLeft.png - blank brown Restart button.
SecondaryButtonRight.png - blank brown Main Menu button.

IMPORT
Sprite (2D and UI), Single, Full Rect, center pivot, 100 pixels per unit.
Alpha Source Input Texture Alpha, Alpha Is Transparency ON.
Max Size 2048, Compression None, Wrap Clamp, Filter Bilinear.
Mipmaps OFF for initial layout; evaluate mipmaps when testing the floating VR panel at different distances.

LAYOUT
Keep your existing World Space GameplayMenuCanvas outside the scaled XR rig.
Set its RectTransform Width=888 Height=978, scale=.0007 on all axes.
MenuWindow: stretch full canvas, offsets all zero, white full alpha, Image Type Simple. Assign PausePanel sprite. Remove previous Outline component: gold outline is in the artwork. Optional UI Shadow: black alpha0.35, Effect Distance X=6 Y=-10.
Keep Tracked Device Graphic Raycaster and existing XR UI Input Module setup.

Use Unity-Layout.csv for exact reference-unit positions. All text and buttons are direct children of MenuWindow with center anchors and center pivot. Local scales=1, rotations=0.
Buttons: assign corresponding PNG; Image Type Simple, white full alpha, Preserve Aspect ON. Target Graphic references its own Image. Preserve existing onClick connections.
Button tint states: Normal #D9D9D9FF, Highlighted #FFFFFFFF, Pressed #B5A894FF, Selected #D9D9D9FF, Disabled #808080FF. Multiplier1, Fade0.1.
Button TMP labels stretch full button with all offsets zero. Center and middle alignment. Cream #FFF2D8. Font sizes in CSV. Auto Size OFF for initial matching. Disable label raycast targets.
Title PAUSED: bold, centered, vertical color gradient top #FFE58A bottom #E5AA35. Time label TIME REMAINING: cream, character spacing 4. Timer05:00: cream. Set all text to centered middle alignment, raycast target OFF.
Font: use a clean geometric sans serif available in the project. Montserrat Bold for title and timer and Montserrat SemiBold for buttons is a close style match, but the generated reference has no recoverable exact font file. Different fonts may need slight size adjustment.

STATE REUSE
Paused: show Resume, time label TIME REMAINING.
Victory/Failure: hide Resume, change title and time label dynamically. The independent buttons allow recentering the lower row for results.
This pack only changes visuals. Existing ScreenTimer remains the timer source; pause/result/controller and head follow logic are separate.

QUALITY
These assets derive from the approved mockup using text/object removal and traced transparency. Some reconstructed texture differences are possible. Dimensions are original asset dimensions, not artificial12K enlargements.
