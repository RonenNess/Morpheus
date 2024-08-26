using Morpheus;
using Morpheus.Demo.RayLib;
using Raylib_cs;
using static Raylib_cs.Raylib;

// init window
Raylib.SetConfigFlags(ConfigFlags.Msaa4xHint | ConfigFlags.ResizableWindow);
InitWindow(0, 0, "Morpheus Demo - RayLib");
MaximizeWindow();
SetTargetFPS(0);

// get screen resolution 
int screenWidth = Raylib_cs.Raylib.GetMonitorWidth(0);
int screenHeight = Raylib_cs.Raylib.GetMonitorHeight(0);

// list of animated objects and their animations
List<(AnimatedObject Data, AnimationBuilder Animation)> animatedObjects = new List<(AnimatedObject obj, AnimationBuilder animation)>();

// which components to animate
bool animatePosition = true;
bool animateColor = true;
bool animateScale = true;
float animationSpeed = 0.5f;

// build all objects and animators
var interpolationMethods = typeof(Interpolation).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
foreach (var inter in interpolationMethods)
{
    InterpolateMethod interpolateMethod =
            (InterpolateMethod)Delegate.CreateDelegate(typeof(InterpolateMethod), inter);

    var target = new AnimatedObject() { InterpolationName = inter.Name };

    var animationBuilder = Morpheus.Morpheus
        .Animate(target)
            .Property("Offset").From(0f).To((screenWidth * 0.35f), interpolateMethod)
            .Property("Scale").From(0.5f).To(1.5f, interpolateMethod)
            .Property("Color").From(System.Drawing.Color.Red).To(System.Drawing.Color.Green, interpolateMethod)
        .For(TimeSpan.FromSeconds(1f));

    animatedObjects.Add((target, animationBuilder));
}

// Main game loop
while (!WindowShouldClose())
{
    // begin drawing
    BeginDrawing();
    ClearBackground(Color.DarkBlue);

    // update morpheus manually
    Morpheus.Morpheus.Update(GetFrameTime());

    // draw animations
    {
        BeginMode2D(new Camera2D() { Zoom = 1f });
        int index = 0;
        int offsetX = 40;
        float gridSizeY = (screenHeight - 160) / (animatedObjects.Count / 2);
        float size = gridSizeY * 0.35f;
        foreach (var animation in animatedObjects)
        {
            var offsetY = 50 + gridSizeY * index;
            var animatedColor = animateColor ? new Color(animation.Data.Color.R, animation.Data.Color.G, animation.Data.Color.B, animation.Data.Color.A) : new Color(255, 0, 0, 255);
            var animatedOffset = animatePosition ? animation.Data.Offset : 0;
            var animatedScale = animateScale ? animation.Data.Scale : 1f;
            Raylib.DrawCircle(offsetX + (int)(size + animatedOffset), (int)(offsetY + size * 1.75f), (size / 2) * animatedScale, animatedColor);
            Raylib.DrawText(animation.Data.InterpolationName, offsetX + 10, (int)(offsetY) + 16, 20, Color.Black);
            Raylib.DrawText(animation.Data.InterpolationName, offsetX + 12, (int)(offsetY) + 18, 20, Color.White);
            if ((offsetY + 250) >= screenHeight)
            {
                offsetX += screenWidth / 2;
                index = -1;
            }
            index++;
        }
        EndMode2D();
    }

    // draw instructions
    {
        var instructions = $"Press 'Space' to animate: Colors [c] = {animateColor} | Position [p] = {animatePosition} | Scale [s] = {animateScale} | Speed [+ / -] = {animationSpeed}.";
        Raylib.DrawText(instructions, 10, 10, 26, Color.Black);
        Raylib.DrawText(instructions, 12, 12, 26, Color.White);
    }

    // toggle animation types and change speed
    {
        animationSpeed = MathF.Round(animationSpeed, 2);
        if (animationSpeed < 2f && Raylib.IsKeyPressed(KeyboardKey.Equal)) { animationSpeed += 0.1f; }
        if (animationSpeed > 0.1f && Raylib.IsKeyPressed(KeyboardKey.Minus)) { animationSpeed -= 0.1f; }
        if (Raylib.IsKeyPressed(KeyboardKey.C)) { animateColor = !animateColor; }
        if (Raylib.IsKeyPressed(KeyboardKey.P)) { animatePosition = !animatePosition; }
        if (Raylib.IsKeyPressed(KeyboardKey.S)) { animateScale = !animateScale; }
    }

    // play animations
    if (Raylib.IsKeyPressed(KeyboardKey.Space))
    {
        Morpheus.Morpheus.RemoveAll();
        foreach (var animation in animatedObjects)
        {
            animation.Animation.Start(animationSpeed);
        }
    }

    // end drawing
    EndDrawing();
}

CloseWindow();
Environment.Exit(0);