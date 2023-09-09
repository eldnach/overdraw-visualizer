using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class overdrawViz : MonoBehaviour
{
    public Shader shader;

    Camera cam;

    int amountX;
    int amountY;

    ComputeShader computeShader;
    ComputeBuffer buffer0;
    RenderTexture overdrawRT;

    public int minRange;
    public int maxRange;

    int[] counter;

    int average;
    int max;

    public GUISkin guiSkin;
    public bool enableGUI;

    struct ComputeKernel
    {
        public int[] workGroupSize;
        public int[] workGroupAmount;
        public int kernelID;
    }

    ComputeKernel computeKernel;
    ComputeKernel computeKernelTriangles;

    void InitializeComputeBuffers()
    {
        counter = new int[amountX * amountY];

        for (int i = 0; i < amountX; i++)
        {
            for (int j = 0; j < amountY; j++)
            {
                counter[j * amountX + i] = 0;
            }
        }

        int stride = System.Runtime.InteropServices.Marshal.SizeOf(typeof(int));
        buffer0 = new ComputeBuffer(amountX * amountY, stride, ComputeBufferType.Default);
        buffer0.SetData(counter);
    }

    void DownloadData()
    {
        buffer0.GetData(counter);
        int sum = 0;
        for (int i = 0; i < amountX; i++)
        {
            for (int j = 0; j < amountY; j++)
            {
               sum += counter[j * amountX + i];
            }
        }
        average = sum / (amountY * amountY);
        Debug.Log("Average overdraw count per pixel " + average);

    }

    // Start is called before the first frame update
    void Start()
    {
        amountX = Screen.width;
        amountY = Screen.height;
        max = 0;
        average = 0;

        overdrawRT = new RenderTexture(amountX, amountY, 24);
        overdrawRT.enableRandomWrite = true;
        overdrawRT.Create();

        computeShader = Resources.Load<ComputeShader>("ComputeOverdraw");
        computeKernel = new ComputeKernel();
        computeKernel.workGroupSize = new int[3] { 8, 8, 1 };
        computeKernel.workGroupAmount = new int[3]
        {
            Mathf.CeilToInt(amountX / computeKernel.workGroupSize[0]),
            Mathf.CeilToInt(amountY / computeKernel.workGroupSize[1]),
            1
        };
        computeKernel.kernelID = computeShader.FindKernel("Overdraw");

        InitializeComputeBuffers();

        // Before drawing, replace all shaders with "overdraw" shader
        cam = gameObject.GetComponent<Camera>();
        cam.SetReplacementShader(shader, "RenderType");

        //camera.SetReplacementShader(transparentShader, "opaqueTag");

    }

    void OnDestroy()
    {
        buffer0.Dispose();
    }


    // 1. After rendering to overdraw texture to framebuffer, use framebuffer as input for compute shader
    // 2. Compute shader calculates "overdraw count" from overdraw texture, and outputs to a color target (overdrawRT)
    // 3. overdrawRT gets blit to framebuffer
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {

        computeShader.SetTexture(computeKernel.kernelID, "_overdrawMap", src, 0, UnityEngine.Rendering.RenderTextureSubElement.Color);
        computeShader.SetTexture(computeKernel.kernelID, "_colorMap", overdrawRT, 0, UnityEngine.Rendering.RenderTextureSubElement.Color);
        counter[0] = 0;
        buffer0.SetData(counter);
        computeShader.SetBuffer(computeKernel.kernelID, "_counter", buffer0);

        computeShader.SetInt( "_rangeMin", minRange);
        computeShader.SetInt( "_rangeMax", maxRange);
        computeShader.SetInt("_totalX", amountX);
        computeShader.SetInt("_totalY", amountY);
        computeShader.Dispatch(computeKernel.kernelID, computeKernel.workGroupAmount[0], computeKernel.workGroupAmount[1], computeKernel.workGroupAmount[2]);

        Graphics.Blit(overdrawRT, dest);


    }

    void OnGUI()
    {
        if (enableGUI)
        {
            float xmargin = 30.0f;
            float ymargin = 30.0f;

            int width = amountX / 8;
            int height = amountY / 25;
            GUI.skin = guiSkin;
            Rect rect = new Rect(xmargin, ymargin, width, height);

            rect = new Rect(xmargin, ymargin - 20.0f, width, height * 10.0f);
            GUI.Label(rect, "Overdraw Visualizer");

            rect = new Rect(xmargin, ymargin + height * 0.0f, width, height * .5f);
            minRange = guiWidgets.iSlider(rect, minRange, 0, 10, "Minimum overdraw range (" + minRange.ToString() + ")");

            rect = new Rect(xmargin, ymargin + height * 1.0f, width, height * .5f);
            maxRange = guiWidgets.iSlider(rect, maxRange, 0, 10, "Maximum overdraw range (" + maxRange.ToString() + ")");

            rect = new Rect(xmargin, ymargin + height * 2.25f, width, height * .5f);
            if (GUI.Button(rect, "Download overdraw counters"))
            {
                DownloadData();
            }
            rect = new Rect(xmargin, ymargin + height * 3.0f, width, height * .5f);
            GUI.Label(rect, "Average overdraw count: " + average.ToString());

            rect = new Rect(xmargin, ymargin + height * 3.5f, width, height * .5f);
            GUI.Label(rect, "Maximum overdraw count: " + max.ToString());


        }

    }

}
