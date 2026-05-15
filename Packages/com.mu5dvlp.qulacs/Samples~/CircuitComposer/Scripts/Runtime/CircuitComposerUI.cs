using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircuitComposerUI : MonoBehaviour
{
    CircuitComposer composer;

    const float CellSize = 56f;
    const float CellGap = 4f;
    const float LabelW = 40f;
    const float ToolH = 38f;
    const float ToolBtnW = 32f;
    const float BarMaxW = 280f;
    const float RowH = 22f;
    const int MaxResults = 32;

    static readonly Color cBg = new(0.12f, 0.12f, 0.15f);
    static readonly Color cPanel = new(0.18f, 0.18f, 0.22f);
    static readonly Color cEmpty = new(0.25f, 0.25f, 0.30f);
    static readonly Color cWire = new(0.4f, 0.4f, 0.45f);
    static readonly Color cText = new(0.9f, 0.9f, 0.92f);
    static readonly Color cDim = new(0.63f, 0.63f, 0.67f);
    static readonly Color cBar = new(0.31f, 0.71f, 1f);
    static readonly Color cBarQ = new(0.35f, 0.85f, 0.5f);

    static Color GClr(ComposerGate g) =>
        g switch
        {
            ComposerGate.H => new(1f, 0.85f, 0.2f),
            ComposerGate.X => new(1f, 0.3f, 0.3f),
            ComposerGate.Y => new(0.3f, 0.9f, 0.4f),
            ComposerGate.Z => new(0.3f, 0.5f, 1f),
            ComposerGate.S => new(0.2f, 0.8f, 0.85f),
            ComposerGate.T => new(0.8f, 0.35f, 0.9f),
            ComposerGate.CNOT => new(0.9f, 0.6f, 0.2f),
            ComposerGate.SWAP => new(0.7f, 0.4f, 0.9f),
            ComposerGate.Measure => new(0.6f, 0.6f, 0.65f),
            ComposerGate.None => new(0.5f, 0.2f, 0.2f),
            _ => cEmpty,
        };

    static string GSym(ComposerGate g, bool ctrl) =>
        g switch
        {
            ComposerGate.H => "H",
            ComposerGate.X => "X",
            ComposerGate.Y => "Y",
            ComposerGate.Z => "Z",
            ComposerGate.S => "S",
            ComposerGate.T => "T",
            ComposerGate.CNOT => ctrl ? "●" : "⊕",
            ComposerGate.SWAP => "×",
            ComposerGate.Measure => "M",
            ComposerGate.None => "✕",
            _ => "",
        };

    RectTransform gridContent,
        linesLayer,
        resultsContent;
    Text statusText,
        infoText;
    readonly Dictionary<ComposerGate, Image> toolImgs = new();
    Image[,] cellImgs;
    Text[,] cellTexts;
    readonly List<GameObject> connLines = new();
    readonly List<GameObject> resultObjs = new();

    Font font;

    public void Init(CircuitComposer c)
    {
        composer = c;
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (font == null)
            font = Font.CreateDynamicFontFromOSFont("Arial", 14);
        Build();
    }

    void Build()
    {
        var cvs = CreateCanvas();
        var root = Rt("Root", cvs.transform);
        Stretch(root);
        Img(root, cBg);
        var vl = root.gameObject.AddComponent<VerticalLayoutGroup>();
        vl.padding = new RectOffset(8, 8, 8, 8);
        vl.spacing = 6;
        vl.childForceExpandWidth = true;
        vl.childForceExpandHeight = false;
        vl.childControlWidth = true;
        vl.childControlHeight = true;

        BuildToolbar(root);
        BuildGrid(root);
        BuildResults(root);
        OnToolChanged();
    }

    Canvas CreateCanvas()
    {
        var go = new GameObject("ComposerCanvas");
        go.transform.SetParent(transform, false);
        var c = go.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 100;
        go.AddComponent<GraphicRaycaster>();
        var sc = go.AddComponent<CanvasScaler>();
        sc.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        sc.referenceResolution = new Vector2(1280, 720);
        sc.matchWidthOrHeight = 0.5f;
        return c;
    }

    void BuildToolbar(RectTransform parent)
    {
        var bar = Rt("Toolbar", parent);
        Img(bar, cPanel);
        Lay(bar, prefH: ToolH);
        var hl = bar.gameObject.AddComponent<HorizontalLayoutGroup>();
        hl.padding = new RectOffset(6, 6, 3, 3);
        hl.spacing = 3;
        hl.childAlignment = TextAnchor.MiddleLeft;
        hl.childForceExpandWidth = false;
        hl.childForceExpandHeight = false;
        hl.childControlWidth = true;
        hl.childControlHeight = true;

        ComposerGate[] tools =
        {
            ComposerGate.H,
            ComposerGate.X,
            ComposerGate.Y,
            ComposerGate.Z,
            ComposerGate.S,
            ComposerGate.T,
            ComposerGate.CNOT,
            ComposerGate.SWAP,
            ComposerGate.Measure,
            ComposerGate.None,
        };
        foreach (var g in tools)
            MakeToolBtn(bar, g);

        MakeSep(bar);

        MakeRunBtn(bar);

        Lay(Rt("Sp", bar), flexW: 1);

        MakeActBtn(bar, "+Q", () => composer.AddQubit());
        MakeActBtn(bar, "-Q", () => composer.RemoveQubit());
        MakeActBtn(bar, "+S", () => composer.AddStep());
        MakeActBtn(bar, "-S", () => composer.RemoveStep());

        MakeSep(bar);

        MakeActBtn(bar, "CLR", () => composer.ClearAll());

        var stRt = Rt("Status", bar);
        Lay(stRt, prefW: 160);
        statusText = Txt(stRt, "", 11, TextAnchor.MiddleLeft, cDim);
    }

    void MakeToolBtn(RectTransform parent, ComposerGate gate)
    {
        var rt = Rt($"T_{gate}", parent);
        Lay(rt, prefW: ToolBtnW, prefH: ToolBtnW);
        var img = Img(rt, GClr(gate));
        toolImgs[gate] = img;
        var txt = Txt(rt, GSym(gate, false), 15, TextAnchor.MiddleCenter, Color.white);
        txt.fontStyle = FontStyle.Bold;
        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.transition = Selectable.Transition.None;
        var g = gate;
        btn.onClick.AddListener(() => composer.SelectTool(g));
    }

    void MakeActBtn(RectTransform parent, string label, UnityEngine.Events.UnityAction act)
    {
        var rt = Rt($"A_{label}", parent);
        Lay(rt, prefW: ToolBtnW, prefH: ToolBtnW);
        var img = Img(rt, new Color(0.3f, 0.3f, 0.37f));
        Txt(rt, label, 12, TextAnchor.MiddleCenter, cText);
        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(act);
    }

    void MakeRunBtn(RectTransform parent)
    {
        var rt = Rt("A_RUN", parent);
        Lay(rt, prefW: ToolBtnW * 1.8f, prefH: ToolBtnW);
        var img = Img(rt, new Color(0.2f, 0.65f, 0.3f));
        var txt = Txt(rt, "RUN", 13, TextAnchor.MiddleCenter, Color.white);
        txt.fontStyle = FontStyle.Bold;
        var btn = rt.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        btn.onClick.AddListener(() => composer.RunCircuit());
    }

    void MakeSep(RectTransform parent)
    {
        var sep = Rt("Sep", parent);
        Lay(sep, prefW: 2);
        Img(sep, cWire);
    }

    void BuildGrid(RectTransform parent)
    {
        var scrollRt = Rt("GridScroll", parent);
        Img(scrollRt, cPanel);
        Lay(scrollRt, flexH: 1);
        var sr = scrollRt.gameObject.AddComponent<ScrollRect>();
        sr.horizontal = true;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Clamped;

        var vp = Rt("VP", scrollRt);
        Stretch(vp);
        Img(vp, cPanel);
        vp.gameObject.AddComponent<Mask>().showMaskGraphic = true;
        sr.viewport = vp;

        gridContent = Rt("Content", vp);
        gridContent.anchorMin = new Vector2(0, 1);
        gridContent.anchorMax = new Vector2(0, 1);
        gridContent.pivot = new Vector2(0, 1);
        sr.content = gridContent;

        PopulateGrid();
    }

    void PopulateGrid()
    {
        int nq = composer.QubitCount,
            ns = composer.StepCount;
        float w = LabelW + ns * (CellSize + CellGap);
        float h = nq * (CellSize + CellGap);
        gridContent.sizeDelta = new Vector2(w, h);

        cellImgs = new Image[nq, ns];
        cellTexts = new Text[nq, ns];

        for (int q = 0; q < nq; q++)
        {
            float cy = -(q * (CellSize + CellGap) + CellSize * 0.5f);
            var wire = Rt($"W{q}", gridContent);
            wire.anchorMin = wire.anchorMax = new Vector2(0, 1);
            wire.pivot = new Vector2(0, 0.5f);
            wire.anchoredPosition = new Vector2(LabelW, cy);
            wire.sizeDelta = new Vector2(ns * (CellSize + CellGap) - CellGap, 2);
            Img(wire, cWire);
        }

        for (int q = 0; q < nq; q++)
        {
            var lrt = Rt($"L{q}", gridContent);
            lrt.anchorMin = lrt.anchorMax = new Vector2(0, 1);
            lrt.pivot = new Vector2(0, 1);
            lrt.anchoredPosition = new Vector2(0, -(q * (CellSize + CellGap)));
            lrt.sizeDelta = new Vector2(LabelW, CellSize);
            Txt(lrt, $"q{q}", 14, TextAnchor.MiddleCenter, cText);
        }

        for (int q = 0; q < nq; q++)
        for (int s = 0; s < ns; s++)
        {
            float x = LabelW + s * (CellSize + CellGap);
            float y = -(q * (CellSize + CellGap));
            var crt = Rt($"C{q}_{s}", gridContent);
            crt.anchorMin = crt.anchorMax = new Vector2(0, 1);
            crt.pivot = new Vector2(0, 1);
            crt.anchoredPosition = new Vector2(x, y);
            crt.sizeDelta = new Vector2(CellSize, CellSize);

            cellImgs[q, s] = Img(crt, cEmpty);
            var txt = Txt(crt, "", 20, TextAnchor.MiddleCenter, Color.white);
            txt.fontStyle = FontStyle.Bold;
            cellTexts[q, s] = txt;

            var btn = crt.gameObject.AddComponent<Button>();
            btn.targetGraphic = cellImgs[q, s];
            btn.transition = Selectable.Transition.ColorTint;
            var cb = btn.colors;
            cb.highlightedColor = new Color(0.85f, 0.85f, 0.85f);
            cb.pressedColor = new Color(0.6f, 0.6f, 0.6f);
            btn.colors = cb;

            int cq = q,
                cs = s;
            btn.onClick.AddListener(() => composer.OnCellClicked(cq, cs));

            RefreshCellVisual(q, s);
        }

        linesLayer = Rt("Lines", gridContent);
        linesLayer.anchorMin = linesLayer.anchorMax = new Vector2(0, 1);
        linesLayer.pivot = new Vector2(0, 1);
        linesLayer.sizeDelta = gridContent.sizeDelta;
        var cg = linesLayer.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.interactable = false;

        RefreshConnections();
    }

    void BuildResults(RectTransform parent)
    {
        var panel = Rt("Results", parent);
        Img(panel, cPanel);
        Lay(panel, prefH: 200);

        var hdr = Rt("Hdr", panel);
        hdr.anchorMin = new Vector2(0, 1);
        hdr.anchorMax = new Vector2(1, 1);
        hdr.pivot = new Vector2(0, 1);
        hdr.sizeDelta = new Vector2(0, 22);
        hdr.offsetMin = new Vector2(8, hdr.offsetMin.y);
        var ht = Txt(hdr, "State Probabilities", 13, TextAnchor.MiddleLeft, cText);
        ht.fontStyle = FontStyle.Bold;

        var srRt = Rt("RScroll", panel);
        srRt.anchorMin = Vector2.zero;
        srRt.anchorMax = Vector2.one;
        srRt.offsetMin = new Vector2(4, 24);
        srRt.offsetMax = new Vector2(-4, -24);
        var sr = srRt.gameObject.AddComponent<ScrollRect>();
        sr.horizontal = false;
        sr.vertical = true;
        sr.movementType = ScrollRect.MovementType.Clamped;

        var vp = Rt("VP", srRt);
        Stretch(vp);
        Img(vp, cPanel);
        vp.gameObject.AddComponent<Mask>().showMaskGraphic = true;
        sr.viewport = vp;

        resultsContent = Rt("RC", vp);
        resultsContent.anchorMin = new Vector2(0, 1);
        resultsContent.anchorMax = new Vector2(1, 1);
        resultsContent.pivot = new Vector2(0, 1);
        sr.content = resultsContent;

        var infoRt = Rt("Info", panel);
        infoRt.anchorMin = Vector2.zero;
        infoRt.anchorMax = new Vector2(1, 0);
        infoRt.pivot = new Vector2(0, 0);
        infoRt.sizeDelta = new Vector2(0, 22);
        infoRt.offsetMin = new Vector2(8, 0);
        infoText = Txt(infoRt, "", 11, TextAnchor.MiddleLeft, cDim);
    }

    // --- Public API called by CircuitComposer ---

    public void RefreshCell(int q, int s)
    {
        if (cellImgs == null || q >= cellImgs.GetLength(0) || s >= cellImgs.GetLength(1))
            return;
        RefreshCellVisual(q, s);
    }

    public void RefreshConnections()
    {
        foreach (var go in connLines)
            Destroy(go);
        connLines.Clear();
        if (linesLayer == null)
            return;

        for (int s = 0; s < composer.StepCount; s++)
        for (int q = 0; q < composer.QubitCount; q++)
        {
            var c = composer.GetCell(q, s);
            if (!c.IsTwoQubit || !c.isControl || c.partnerQubit < 0)
                continue;

            int minQ = Mathf.Min(q, c.partnerQubit);
            int maxQ = Mathf.Max(q, c.partnerQubit);
            float cx = LabelW + s * (CellSize + CellGap) + CellSize * 0.5f;
            float y1 = minQ * (CellSize + CellGap) + CellSize * 0.5f;
            float y2 = maxQ * (CellSize + CellGap) + CellSize * 0.5f;

            var lr = Rt($"Ln{q}_{s}", linesLayer);
            lr.anchorMin = lr.anchorMax = new Vector2(0, 1);
            lr.pivot = new Vector2(0.5f, 1);
            lr.anchoredPosition = new Vector2(cx, -y1);
            lr.sizeDelta = new Vector2(3, y2 - y1);
            Img(lr, GClr(c.gate));
            connLines.Add(lr.gameObject);
        }
    }

    public void OnToolChanged()
    {
        foreach (var kv in toolImgs)
        {
            var c = GClr(kv.Key);
            kv.Value.color = kv.Key == composer.SelectedTool ? c : new Color(c.r * 0.35f, c.g * 0.35f, c.b * 0.35f, 1f);
        }
    }

    public void ShowStatus(string msg)
    {
        if (statusText != null)
            statusText.text = msg;
    }

    public void ShowResults(double[] probs, double[] qubitP1, int depth, int gates)
    {
        foreach (var go in resultObjs)
            Destroy(go);
        resultObjs.Clear();

        var entries = new List<(int i, double p)>();
        for (int i = 0; i < probs.Length; i++)
            if (probs[i] > 0.0005)
                entries.Add((i, probs[i]));
        entries.Sort((a, b) => b.p.CompareTo(a.p));
        if (entries.Count > MaxResults)
            entries.RemoveRange(MaxResults, entries.Count - MaxResults);

        int nq = composer.QubitCount;
        float y = 0;

        foreach (var (idx, prob) in entries)
        {
            MakeResultRow(BasisLabel(idx, nq), prob, y, cBar);
            y -= RowH + 2;
        }

        y -= 6;
        for (int q = 0; q < nq; q++)
        {
            MakeResultRow($"q{q} P(|1>)", qubitP1[q], y, cBarQ);
            y -= RowH + 2;
        }

        resultsContent.sizeDelta = new Vector2(0, -y + 8);
        if (infoText != null)
            infoText.text = $"Depth: {depth}  |  Gates: {gates}  |  Qubits: {nq}";
    }

    public void RebuildAll()
    {
        if (gridContent == null)
            return;
        for (int i = gridContent.childCount - 1; i >= 0; i--)
            Destroy(gridContent.GetChild(i).gameObject);
        connLines.Clear();
        PopulateGrid();
    }

    // --- Internal ---

    void RefreshCellVisual(int q, int s)
    {
        var cell = composer.GetCell(q, s);
        cellImgs[q, s].color = cell.IsEmpty ? cEmpty : GClr(cell.gate);
        cellTexts[q, s].text = cell.IsEmpty ? "" : GSym(cell.gate, cell.isControl);
    }

    void MakeResultRow(string label, double prob, float y, Color barColor)
    {
        var row = Rt("R", resultsContent);
        row.anchorMin = new Vector2(0, 1);
        row.anchorMax = new Vector2(1, 1);
        row.pivot = new Vector2(0, 1);
        row.anchoredPosition = new Vector2(0, y);
        row.sizeDelta = new Vector2(0, RowH);
        resultObjs.Add(row.gameObject);

        var lrt = Rt("L", row);
        lrt.anchorMin = lrt.anchorMax = new Vector2(0, 0.5f);
        lrt.pivot = new Vector2(1, 0.5f);
        lrt.anchoredPosition = new Vector2(88, 0);
        lrt.sizeDelta = new Vector2(80, RowH);
        Txt(lrt, label, 12, TextAnchor.MiddleRight, cText);

        var bgr = Rt("Bg", row);
        bgr.anchorMin = new Vector2(0, 0.1f);
        bgr.anchorMax = new Vector2(0, 0.9f);
        bgr.pivot = new Vector2(0, 0.5f);
        bgr.anchoredPosition = new Vector2(96, 0);
        bgr.sizeDelta = new Vector2(BarMaxW, 0);
        Img(bgr, new Color(0.15f, 0.15f, 0.2f));

        var br = Rt("Bar", bgr);
        br.anchorMin = Vector2.zero;
        br.anchorMax = new Vector2(Mathf.Clamp01((float)prob), 1);
        br.offsetMin = br.offsetMax = Vector2.zero;
        Img(br, barColor);

        var prt = Rt("P", row);
        prt.anchorMin = prt.anchorMax = new Vector2(0, 0.5f);
        prt.pivot = new Vector2(0, 0.5f);
        prt.anchoredPosition = new Vector2(96 + BarMaxW + 6, 0);
        prt.sizeDelta = new Vector2(60, RowH);
        Txt(prt, $"{prob * 100:F1}%", 11, TextAnchor.MiddleLeft, cDim);
    }

    // --- UI Factory Helpers ---

    static string BasisLabel(int idx, int nq)
    {
        var b = new char[nq];
        for (int i = 0; i < nq; i++)
            b[nq - 1 - i] = ((idx >> i) & 1) == 1 ? '1' : '0';
        return $"|{new string(b)}>";
    }

    static RectTransform Rt(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static Image Img(RectTransform rt, Color c)
    {
        var img = rt.gameObject.AddComponent<Image>();
        img.color = c;
        return img;
    }

    Text Txt(RectTransform rt, string text, int size, TextAnchor align, Color color)
    {
        var go = new GameObject("T", typeof(RectTransform));
        go.transform.SetParent(rt, false);
        Stretch(go.GetComponent<RectTransform>());
        var t = go.AddComponent<Text>();
        t.text = text;
        t.fontSize = size;
        t.alignment = align;
        t.color = color;
        t.font = font;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        return t;
    }

    static void Lay(RectTransform rt, float prefW = -1, float prefH = -1, float flexW = -1, float flexH = -1)
    {
        var le = rt.gameObject.AddComponent<LayoutElement>();
        if (prefW >= 0)
            le.preferredWidth = prefW;
        if (prefH >= 0)
            le.preferredHeight = prefH;
        if (flexW >= 0)
            le.flexibleWidth = flexW;
        if (flexH >= 0)
            le.flexibleHeight = flexH;
    }
}
