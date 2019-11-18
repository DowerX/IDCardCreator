using System;
using SDL2;
using System.Drawing;

namespace sdlidcard
{
    class Program
    {
        static IntPtr zero;
        static bool done;

        static int mousex = 0, mousey = 0;
        static float cutasp;
        static float facescale;
        static bool keepasp = true;

        static SDL.SDL_Rect facesrc = new SDL.SDL_Rect();
        static SDL.SDL_Rect facesrcog = new SDL.SDL_Rect();
        static SDL.SDL_Rect facedrc = new SDL.SDL_Rect();
        static bool getsrc = false;
        static bool getdrc = false;

        static IntPtr facewindow;
        static IntPtr facerenderer;
        static IntPtr fullwindow;
        static IntPtr fullrenderer;
        static IntPtr bgtexture;
        static SDL.SDL_Rect bgsize;
        static IntPtr facetexture;
        static SDL.SDL_Rect facesize;
        static IntPtr facecuttexture;
        static SDL.SDL_Rect facecutsize;

        static void Main(string[] args)
        {
            SDL.SDL_Init(SDL.SDL_INIT_VIDEO);
            zero = IntPtr.Zero;
            done = false;

            (facewindow, facerenderer) = CreateWindow("face", new SDL.SDL_Rect());
            (fullwindow, fullrenderer) = CreateWindow("full", new SDL.SDL_Rect());

            (bgtexture, bgsize) = Load(args[0], fullrenderer);
            (facetexture, facesize) = Load(args[1], facerenderer);
            (facecuttexture, facecutsize) = Load(args[1], fullrenderer);

            SDL.SDL_SetWindowSize(facewindow, 540, 540*facesize.h / facesize.w);
            SDL.SDL_SetWindowSize(fullwindow, 1113, 720);

            SDL.SDL_RenderCopy(facerenderer, facetexture, zero, zero);

            mousex = 0;
            mousey = 0;
            facescale = (float)facesize.w / (float)540;

            getsrc = false;
            getdrc = false;

            while (!done)
            {
                SDL.SDL_RenderClear(fullrenderer);
                SDL.SDL_RenderCopy(fullrenderer, bgtexture, zero, zero);
                SDL.SDL_RenderCopy(fullrenderer, facecuttexture, ref facesrc, ref facedrc);
                SDL.SDL_RenderPresent(facerenderer);
                SDL.SDL_RenderPresent(fullrenderer);

                //Input
                #region
                while (SDL.SDL_PollEvent(out SDL.SDL_Event e) != 0)
                {
                    switch(e.type)
                    {
                        case SDL.SDL_EventType.SDL_QUIT:
                            SDL.SDL_DestroyRenderer(facerenderer);
                            SDL.SDL_DestroyRenderer(fullrenderer);
                            SDL.SDL_DestroyWindow(facewindow);
                            SDL.SDL_DestroyWindow(fullwindow);
                            SDL.SDL_Quit();
                            return;

                        case SDL.SDL_EventType.SDL_KEYDOWN:
                            switch (e.key.keysym.sym)
                            {
                                case SDL.SDL_Keycode.SDLK_q:
                                    SDL.SDL_DestroyRenderer(facerenderer);
                                    SDL.SDL_DestroyRenderer(fullrenderer);
                                    SDL.SDL_DestroyWindow(facewindow);
                                    SDL.SDL_DestroyWindow(fullwindow);
                                    SDL.SDL_Quit();
                                    return;

                                case SDL.SDL_Keycode.SDLK_x:
                                    SaveScreenshot(args[2]);
                                    break;

                                case SDL.SDL_Keycode.SDLK_k:
                                    Console.WriteLine("Keep aspectratio: " + keepasp.ToString());
                                    keepasp = !keepasp;
                                    break;
                            }
                            break;


                        case SDL.SDL_EventType.SDL_MOUSEMOTION:
                            mousex = e.motion.x;
                            mousey = e.motion.y;
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
                            switch (e.button.button)
                            {
                                case (byte)SDL.SDL_BUTTON_LEFT:
                                    facesrc.x = (int)(mousex*facescale);
                                    facesrcog.x = (mousex);
                                    facesrc.y = (int)(mousey*facescale);
                                    facesrcog.y = (mousey);
                                    getsrc = true;
                                    break;

                                case (byte)SDL.SDL_BUTTON_RIGHT:
                                    facedrc.x = mousex;
                                    facedrc.y = mousey;
                                    getdrc = true;
                                    break;
                            }
                            break;

                        case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
                            switch (e.button.button)
                            {
                                case (byte)SDL.SDL_BUTTON_LEFT:
                                    getsrc = false;
                                    break;

                                case (byte)SDL.SDL_BUTTON_RIGHT:
                                    getdrc = false;
                                    break;
                            }
                            break;
                    }
                }

                if (getsrc) SetSrcSize();
                if (getdrc) SetDrcSize();
                #endregion
            }
        }

        static (IntPtr, IntPtr) CreateWindow(string name, SDL.SDL_Rect size)
        {
            IntPtr window = SDL.SDL_CreateWindow(name,
            SDL.SDL_WINDOWPOS_CENTERED,
            SDL.SDL_WINDOWPOS_CENTERED,
            size.w, size.h,
            SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);

            IntPtr renderer = SDL.SDL_CreateRenderer(window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

            return (window, renderer);
        }

        static (IntPtr, SDL.SDL_Rect) Load(string path, IntPtr renderer)
        {
            IntPtr texture = SDL_image.IMG_LoadTexture(renderer, path);
            SDL.SDL_QueryTexture(texture, out uint a, out int b, out int _w, out int _h);
            return (texture , new SDL.SDL_Rect() { h = _h, w = _w });
        }

        static void SetSrcSize()
        {
            facesrc.w = (int)(MathF.Abs(mousex * facescale - facesrc.x));
            facesrcog.w = (int)(MathF.Abs(mousex - facesrcog.x));
            facesrcog.h = (int)(MathF.Abs(mousey- facesrcog.y));
            facesrc.h = (int)(MathF.Abs(mousey * facescale - facesrc.y));

            cutasp = facesrc.h / MathF.Abs(mousex * facescale - facesrc.x);

            if(keepasp) facedrc.h = (int)MathF.Abs(facedrc.w * cutasp);
        }

        static void SetDrcSize()
        {
            facedrc.w = Math.Abs(mousex - facedrc.x);
            if (keepasp)
            {
                facedrc.h = (int)MathF.Abs(facedrc.w * cutasp);
            }
            else
            {
                facedrc.h = Math.Abs(mousey - facedrc.y);
            }
            
        }

        static void SaveScreenshot(string path)
        {
            int x, y, w, h;
            SDL.SDL_GetWindowPosition(fullwindow, out x, out y);
            SDL.SDL_GetWindowSize(fullwindow, out w, out h);
            Bitmap bitmap = new Bitmap(w,h);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(x, y, 0, 0, new Size(w, h));
                bitmap.Save(path);
            }
            Console.WriteLine("Saved screen to " + path);
        }
    }
}
