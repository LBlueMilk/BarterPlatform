define([
    "esri/Map",
    "esri/views/MapView",
    "esri/widgets/Measurement",
    "esri/widgets/Sketch",
    "esri/layers/GraphicsLayer"
], function (Map, MapView, Measurement, Sketch, GraphicsLayer) {

    // 創建圖層
    const graphicsLayer = new GraphicsLayer();

    // 創建地圖
    const map = new Map({
        basemap: "streets-vector",
        layers: [graphicsLayer]
    });

    // 創建地圖視圖 (以台灣為中心)
    const view = new MapView({
        container: "viewDiv",
        map: map,
        center: [120.9605, 23.6978], // 台灣中心點
        zoom: 7
    });

    // 測量工具
    const measurement = new Measurement({
        view: view
    });

    // 繪圖工具
    const sketch = new Sketch({
        view: view,
        layer: graphicsLayer,
        creationMode: "update",
        availableCreateTools: ["point"], // 僅允許新增點
        visibleElements: {
            createTools: { point: true },
            selectionTools: { "lasso-selection": false, "rectangle-selection": false },
            settingsMenu: false
        }
    });

    let measurementActive = false;
    let drawActive = false;

    // 底圖選擇事件
    document.getElementById("basemapSelect").addEventListener("change", function (event) {
        map.basemap = event.target.value;
    });

    // 滑鼠移動事件 - 顯示座標
    view.on("pointer-move", function (event) {
        const point = view.toMap(event);
        const coordsDiv = document.getElementById("coordsDiv");
        coordsDiv.innerHTML = `
                            滑鼠座標<br>
                            經度: ${point.longitude.toFixed(6)}<br>
                            緯度: ${point.latitude.toFixed(6)}
                        `;
    });

    // Sketch 新增點後觸發資訊面板更新
    sketch.on("create", function (event) {
        if (event.state === "complete" && event.graphic.geometry.type === "point") {
            const lon = event.graphic.geometry.longitude.toFixed(6);
            const lat = event.graphic.geometry.latitude.toFixed(6);
            const featureInfo = document.getElementById("featureInfo");
            const featureDetails = document.getElementById("featureDetails");
            featureDetails.innerHTML = `
                <strong>建立點位置:</strong><br>
                經度: ${lon}<br>
                緯度: ${lat}<br>
                <small>此點可拖曳、刪除、編輯</small>
            `;
            featureInfo.style.display = "block";
        }
    });

    // 全域函數定義
    window.goToLocation = function (location) {
        const locations = {
            taipei: { center: [121.5654, 25.0330], zoom: 11 },
            kaohsiung: { center: [120.3014, 22.6273], zoom: 11 },
            taichung: { center: [120.6736, 24.1477], zoom: 11 },
            taiwan: { center: [120.9605, 23.6978], zoom: 7 }
        };

        const loc = locations[location];
        if (loc) view.goTo(loc);
    };

    window.toggleMeasure = function () {
        const btn = document.getElementById("measureBtn");
        if (!measurementActive) {
            measurement.activeTool = "distance";
            view.ui.add(measurement, "top-right");
            btn.textContent = "停止測距";
            btn.classList.add("active");
            measurementActive = true;
        } else {
            measurement.clear();
            view.ui.remove(measurement);
            btn.textContent = "測距工具";
            btn.classList.remove("active");
            measurementActive = false;
        }
    };

    window.toggleDraw = function () {
        const btn = document.getElementById("drawBtn");
        if (!drawActive) {
            view.ui.add(sketch, "top-right");
            btn.textContent = "停止繪圖";
            btn.classList.add("active");
            drawActive = true;
        } else {
            view.ui.remove(sketch);
            btn.textContent = "繪圖工具";
            btn.classList.remove("active");
            drawActive = false;
        }
    };

    window.clearGraphics = function () {
        graphicsLayer.removeAll();
        document.getElementById("featureInfo").style.display = "none";
        if (measurementActive) {
            measurement.clear();
        }
    };

    // 地圖載入完成後的初始化
    view.when(function () {
        setTimeout(() => {
            view.resize(); // 強制重新計算尺寸
            view.goTo({ center: [120.9605, 23.6978], zoom: 7 }); // 重導一次視角（修正空畫面）
        }, 300); // 延遲 300ms，確保 DOM 已渲染完成

        console.log("ArcGIS 地圖載入完成！");

        //// 添加一些示範圖形
        //const sampleLine = new Graphic({
        //    geometry: {
        //        type: "polyline",
        //        paths: [[120, 23], [121, 24], [122, 23]]
        //    },
        //    symbol: new SimpleLineSymbol({
        //        color: [0, 255, 0],
        //        width: 3
        //    })
        //});

        //graphicsLayer.add(sampleLine);
    });

    // 模組載入完成
    console.log("ArcGISAPI 模組載入完成");
});