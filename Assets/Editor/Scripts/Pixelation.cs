
//using System.Collections.Generic;
//using System.IO;
//using System.Runtime.InteropServices;
//using UnityEditor;
//using UnityEngine;

//[RequireComponent(typeof(Camera))]
//[ExecuteInEditMode]
//[AddComponentMenu("Image Effects/Rendering/Pixelation")]

//public class Pixelation : MonoBehaviour
//{
//    [SerializeField] private ComputeShader pixelateComputeShader = default;
//    [SerializeField] private ComputeShader copyComputeShader = default;

//    [SerializeField] private int targetWidth = default;
//    [SerializeField] private int targetHeight = default;
//    [SerializeField] private int colorLayers = default;
//    [SerializeField] private int cleanUpMaxPixelCount = default;
//    [SerializeField] private string directory = default;
//    [SerializeField] private string fileName = default;
//    [SerializeField] private bool save = default;

//    private const int THREADS = 32;

//    private int pixelateKernel;
//    private int calculateHorizontalEdgesKernel;
//    private int calculateVerticalEdgesKernel;
//    private int copyKernel;
//    private int cleanUpKernel;

//    private RenderTexture screenTexture;
//    private RenderTexture pixelTexture;
//    private ComputeBuffer leftCutBuffer;
//    private ComputeBuffer rightCutBuffer;
//    private ComputeBuffer topCutBuffer;
//    private ComputeBuffer bottomCutBuffer;
//    private ComputeBuffer pixelCheckBuffer;
//    private Camera pixelCamera;
//    private float[] data;

//    private void Initialize()
//    {
//        if (pixelTexture != null)
//            pixelTexture.Release();

//        if (screenTexture != null)
//            screenTexture.Release();

//        if (leftCutBuffer != null)
//            leftCutBuffer.Release();

//        if (rightCutBuffer != null)
//            rightCutBuffer.Release();

//        if (topCutBuffer != null)
//            topCutBuffer.Release();

//        if (bottomCutBuffer != null)
//            bottomCutBuffer.Release();

//        if (pixelCheckBuffer != null)
//            pixelCheckBuffer.Release();

//        leftCutBuffer = new ComputeBuffer(Screen.height, Marshal.SizeOf<int>());
//        rightCutBuffer = new ComputeBuffer(Screen.height, Marshal.SizeOf<int>());
//        topCutBuffer = new ComputeBuffer(Screen.width, Marshal.SizeOf<int>());
//        bottomCutBuffer = new ComputeBuffer(Screen.width, Marshal.SizeOf<int>());
//        pixelCheckBuffer = new ComputeBuffer(targetWidth * targetHeight, Marshal.SizeOf<int>());

//        data = new float[6];
//        pixelCamera = GetComponent<Camera>();
//        screenTexture = new RenderTexture(Screen.width, Screen.height, 0);
//        screenTexture.wrapMode = TextureWrapMode.Clamp;
//        screenTexture.filterMode = FilterMode.Point;
//        screenTexture.enableRandomWrite = true;
//        screenTexture.Create();

//        pixelTexture = new RenderTexture(targetWidth, targetHeight, 0);
//        pixelTexture.wrapMode = TextureWrapMode.Clamp;
//        pixelTexture.filterMode = FilterMode.Point;
//        pixelTexture.enableRandomWrite = true;
//        pixelTexture.Create();

//        copyKernel = copyComputeShader.FindKernel("Copy");
//        copyComputeShader.SetInt("sourceWidth", targetWidth);
//        copyComputeShader.SetInt("sourceHeight", targetHeight);
//        copyComputeShader.SetInt("targetXPosition", Screen.width - targetWidth - 2);
//        copyComputeShader.SetInt("targetYPosition", 2);
//        copyComputeShader.SetTexture(copyKernel, "source", pixelTexture);
//        copyComputeShader.SetTexture(copyKernel, "target", screenTexture);

//        pixelateKernel = pixelateComputeShader.FindKernel("Pixelate");
//        pixelateComputeShader.SetInt("colorLayers", colorLayers);
//        pixelateComputeShader.SetInt("sourceWidth", Screen.width);
//        pixelateComputeShader.SetInt("sourceHeight", Screen.height);
//        pixelateComputeShader.SetInt("targetWidth", targetWidth);
//        pixelateComputeShader.SetInt("targetHeight", targetHeight);
//        pixelateComputeShader.SetTexture(pixelateKernel, "source", screenTexture);
//        pixelateComputeShader.SetTexture(pixelateKernel, "target", pixelTexture);
//        pixelateComputeShader.SetBuffer(pixelateKernel, "pixelCheck", pixelCheckBuffer);

//        calculateHorizontalEdgesKernel = pixelateComputeShader.FindKernel("CalculateHorizontalEdges");
//        pixelateComputeShader.SetTexture(calculateHorizontalEdgesKernel, "source", screenTexture);
//        pixelateComputeShader.SetBuffer(calculateHorizontalEdgesKernel, "leftCut", leftCutBuffer);
//        pixelateComputeShader.SetBuffer(calculateHorizontalEdgesKernel, "rightCut", rightCutBuffer);

//        calculateVerticalEdgesKernel = pixelateComputeShader.FindKernel("CalculateVerticalEdges");
//        pixelateComputeShader.SetTexture(calculateVerticalEdgesKernel, "source", screenTexture);
//        pixelateComputeShader.SetBuffer(calculateVerticalEdgesKernel, "topCut", topCutBuffer);
//        pixelateComputeShader.SetBuffer(calculateVerticalEdgesKernel, "bottomCut", bottomCutBuffer);

//        cleanUpKernel = pixelateComputeShader.FindKernel("CleanUp");
//        pixelateComputeShader.SetBuffer(cleanUpKernel, "pixelCheck", pixelCheckBuffer);
//        pixelateComputeShader.SetTexture(cleanUpKernel, "target", pixelTexture);
//    }

//    private void OnRenderImage(RenderTexture source, RenderTexture destination)
//    {
//        Initialize();

//        Graphics.Blit(source, screenTexture);

//        pixelateComputeShader.Dispatch(calculateHorizontalEdgesKernel, Mathf.CeilToInt((float)Screen.height / THREADS), 1, 1);
//        pixelateComputeShader.Dispatch(calculateVerticalEdgesKernel, Mathf.CeilToInt((float)Screen.width / THREADS), 1, 1);

//        int[] leftCutData = new int[Screen.height];
//        int[] rightCutData = new int[Screen.height];
//        int[] topCutData = new int[Screen.width];
//        int[] bottomCutData = new int[Screen.width];

//        leftCutBuffer.GetData(leftCutData);
//        rightCutBuffer.GetData(rightCutData);
//        topCutBuffer.GetData(topCutData);
//        bottomCutBuffer.GetData(bottomCutData);

//        int xMin = Mathf.Min(leftCutData);
//        int xMax = Screen.width - Mathf.Min(rightCutData);
//        int yMin = Mathf.Min(bottomCutData);
//        int yMax = Screen.height - Mathf.Min(topCutData);

//        if (xMin < xMax && yMin < yMax)
//        {
//            int screenWidth = xMax - xMin + 1;
//            int screenHeight = yMax - yMin + 1;
//            float blendWidth = (float)screenWidth / targetWidth;
//            float blendHeight = (float)screenHeight / targetHeight;

//            int bottomPadding;
//            int leftPadding;
//            if (blendWidth > blendHeight)
//            {
//                pixelateComputeShader.SetFloat("blendSize", blendWidth);
//                bottomPadding = (int)((targetHeight - targetHeight * blendHeight / blendWidth) / 2);
//                leftPadding = 0;
//            }
//            else
//            {
//                pixelateComputeShader.SetFloat("blendSize", blendHeight);
//                leftPadding = (int)((targetWidth - targetWidth * blendWidth / blendHeight) / 2);
//                bottomPadding = 0;
//            }

//            pixelateComputeShader.SetInt("bottomPadding", bottomPadding);
//            pixelateComputeShader.SetInt("leftPadding", leftPadding);
//            pixelateComputeShader.SetInt("screenWidth", screenWidth);
//            pixelateComputeShader.SetInt("screenHeight", screenHeight);
//            pixelateComputeShader.SetInt("sourceXOffset", xMin);
//            pixelateComputeShader.SetInt("sourceYOffset", yMin);

//            int xThreads = Mathf.CeilToInt((float)(targetWidth - leftPadding * 2) / THREADS);
//            int yThreads = Mathf.CeilToInt((float)(targetHeight - bottomPadding * 2) / THREADS);
//            pixelateComputeShader.Dispatch(pixelateKernel, xThreads, yThreads, 1);
//            CleanUp(leftPadding, bottomPadding);

//            xThreads = Mathf.CeilToInt((float)targetWidth / THREADS);
//            yThreads = Mathf.CeilToInt((float)targetHeight / THREADS);
//            pixelateComputeShader.Dispatch(cleanUpKernel, xThreads, yThreads, 1);
//            copyComputeShader.Dispatch(copyKernel, xThreads, yThreads, 1);
//        }
//        Graphics.Blit(screenTexture, destination);

//        Save();
//    }

//    private void CleanUp(int leftPadding, int bottomPadding)
//    {
//        int[] pixelCheck = new int[targetWidth * targetHeight];
//        pixelCheckBuffer.GetData(pixelCheck);
//        bool[] pixelsDone = new bool[targetWidth * targetHeight];

//        for (int i = 0; i < leftPadding; i++)
//        {
//            for (int j = 0; j < targetHeight; j++)
//            {
//                pixelCheck[i + targetWidth * j] = 0;
//                pixelsDone[i + targetWidth * j] = true;

//                pixelCheck[(targetWidth - i - 1) + targetWidth * j] = 0;
//                pixelsDone[(targetWidth - i - 1) + targetWidth * j] = true;
//            }
//        }

//        for (int i = 0; i < bottomPadding; i++)
//        {
//            for (int j = 0; j < targetWidth; j++)
//            {
//                pixelCheck[j + targetWidth * i] = 0;
//                pixelsDone[j + targetWidth * i] = true;

//                pixelCheck[j + targetWidth * (targetHeight - 1 - i)] = 0;
//                pixelsDone[j + targetWidth * (targetHeight - 1 - i)] = true;
//            }
//        }
//        for (int i = leftPadding; i < targetWidth - leftPadding; i++)
//        {
//            for (int j = bottomPadding; j < targetHeight - bottomPadding; j++)
//            {
//                if (pixelsDone[i + targetWidth * j])
//                    continue;

//                List<Point> pixelsChecked = new List<Point>();
//                Queue<Point> pixelsToCheck = new Queue<Point>();
//                Point startPoint = new Point(i, j);
//                pixelsToCheck.Enqueue(startPoint);
//                pixelsChecked.Add(startPoint);
//                pixelsDone[i + targetWidth * j] = true;

//                int count = 1;
//                while (pixelsToCheck.Count > 0)
//                {
//                    Point check = pixelsToCheck.Dequeue();

//                    Point nextCheck = check.AddDirection(DiagonalDirection.CenterLeft);
//                    if (nextCheck.xIndex >= 0 && pixelCheck[nextCheck.xIndex + targetWidth * nextCheck.yIndex] == 1 && !pixelsDone[nextCheck.xIndex + targetWidth * nextCheck.yIndex])
//                    {
//                        pixelsDone[nextCheck.xIndex + targetWidth * nextCheck.yIndex] = true;
//                        pixelsChecked.Add(nextCheck);
//                        pixelsToCheck.Enqueue(nextCheck);
//                        count++;
//                    }

//                    nextCheck = check.AddDirection(DiagonalDirection.CenterRight);
//                    if (nextCheck.xIndex < targetWidth && pixelCheck[nextCheck.xIndex + targetWidth * nextCheck.yIndex] == 1 && !pixelsDone[nextCheck.xIndex + targetWidth * nextCheck.yIndex])
//                    {
//                        pixelsDone[nextCheck.xIndex + targetWidth * nextCheck.yIndex] = true;
//                        pixelsChecked.Add(nextCheck);
//                        pixelsToCheck.Enqueue(nextCheck);
//                        count++;
//                    }

//                    nextCheck = check.AddDirection(DiagonalDirection.BottomCenter);
//                    if (nextCheck.yIndex >= 0 && pixelCheck[nextCheck.xIndex + targetWidth * nextCheck.yIndex] == 1 && !pixelsDone[nextCheck.xIndex + targetWidth * nextCheck.yIndex])
//                    {
//                        pixelsDone[nextCheck.xIndex + targetWidth * nextCheck.yIndex] = true;
//                        pixelsChecked.Add(nextCheck);
//                        pixelsToCheck.Enqueue(nextCheck);
//                        count++;
//                    }

//                    nextCheck = check.AddDirection(DiagonalDirection.TopCenter);
//                    if (nextCheck.yIndex < targetHeight && pixelCheck[nextCheck.xIndex + targetWidth * nextCheck.yIndex] == 1 && !pixelsDone[nextCheck.xIndex + targetWidth * nextCheck.yIndex])
//                    {
//                        pixelsDone[nextCheck.xIndex + targetWidth * nextCheck.yIndex] = true;
//                        pixelsChecked.Add(nextCheck);
//                        pixelsToCheck.Enqueue(nextCheck);
//                        count++;
//                    }
//                }

//                if (count > cleanUpMaxPixelCount)
//                    continue;

//                foreach (Point point in pixelsChecked)
//                    pixelCheck[point.xIndex + targetWidth * point.yIndex] = 0;
//            }
//        }

//        pixelCheckBuffer.SetData(pixelCheck);
//    }

//    private void Save()
//    {
//        if (!save)
//            return;

//        save = false;
//        if (string.IsNullOrEmpty(fileName))
//            return;

//        if (string.IsNullOrEmpty(directory))
//            return;

//        string finalDirectory = "Assets" + "/" + directory;
//        if (!Directory.Exists(finalDirectory))
//            return;

//        RenderTexture.active = pixelTexture;
//        Texture2D tex = new Texture2D(pixelTexture.width, pixelTexture.height, TextureFormat.ARGB32, false);
//        tex.ReadPixels(new Rect(0, 0, pixelTexture.width, pixelTexture.height), 0, 0);
//        RenderTexture.active = null;

//        byte[] bytes = tex.EncodeToPNG();

//        string finalName = finalDirectory + "/" + fileName + ".jpg";
//        File.WriteAllBytes(finalName, bytes);
//        AssetDatabase.ImportAsset(finalName);

//        TextureImporter textureImporter = TextureImporter.GetAtPath(finalName) as TextureImporter;

//        TextureImporterSettings test = new TextureImporterSettings();
//        test.filterMode = FilterMode.Point;
//        test.wrapMode = TextureWrapMode.Clamp;
//        test.textureType = TextureImporterType.Sprite;
//        test.spriteMode = (int)SpriteImportMode.Single;
//        test.textureShape = TextureImporterShape.Texture2D;
//        test.alphaIsTransparency = true;
//        test.alphaSource = TextureImporterAlphaSource.FromInput;
//        test.mipmapEnabled = false;
//        test.readable = true;

//        textureImporter.SetTextureSettings(test);
//        textureImporter.SaveAndReimport();
//    }
//}