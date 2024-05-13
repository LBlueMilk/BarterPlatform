$(document).ready(function () {
    $.ajax({
        type: 'get',
        url: '/api/AgriculturalProductOriginPrices',
        success: (data) => {
            console.log(data);

            let crops = [];
            let years = [];
            let dataItem = "";
            for (let item of data) {
                dataItem += `
                            <tr>
                                <td>${item.作物}</td>
                                <td>${item.年份}</td>
                                <td>${item._1月價格}</td>
                                <td>${item._2月價格}</td>
                                <td>${item._3月價格}</td>
                                <td>${item._4月價格}</td>
                                <td>${item._5月價格}</td>
                                <td>${item._6月價格}</td>
                                <td>${item._7月價格}</td>
                                <td>${item._8月價格}</td>
                                <td>${item._9月價格}</td>
                                <td>${item._10月價格}</td>
                                <td>${item._11月價格}</td>
                                <td>${item._12月價格}</td>
                            </tr>
                        `;

                //將作物加入作物清單
                if (!crops.includes(item.作物)) {
                    crops.push(item.作物);
                }

                //將年份加入年份清單
                if (!years.includes(item.年份)) {
                    years.push(item.年份);
                }
            }

            //將所有的項目添加到表格的 tbody 中
            $('#crop tbody').append(dataItem);

            //在下拉選單中添加作物選項
            for (let crop of crops) {
                $('#cropSelection').append(`<option value="${crop}">${crop}</option>`);
            }

            //在下拉選單中添加年份選項
            for (let year of years) {
                $('#yearSelection').append(`<option value="${year}">${year}</option>`);
            }
        }
    });

    //當選擇作物，執行以下程式碼
    $('#cropSelection').change(function () {
        let selectedCrop = $(this).val(); //取得所選的作物
        $('#crop tbody').empty(); //清空表格

        //再次篩選資料，僅顯示所選作物的資料
        $.ajax({
            type: 'get',
            url: '/api/AgriculturalProductOriginPrices',
            success: (data) => {
                let dataItem = "";
                for (let item of data) {
                    if (item.作物 === selectedCrop || selectedCrop === "") { //如果選擇的作物為空，顯示全部資料。否則僅顯示所選作物的資料
                        dataItem += `
                                    <tr>
                                        <td>${item.作物}</td>
                                        <td>${item.年份}</td>
                                        <td>${item._1月價格}</td>
                                        <td>${item._2月價格}</td>
                                        <td>${item._3月價格}</td>
                                        <td>${item._4月價格}</td>
                                        <td>${item._5月價格}</td>
                                        <td>${item._6月價格}</td>
                                        <td>${item._7月價格}</td>
                                        <td>${item._8月價格}</td>
                                        <td>${item._9月價格}</td>
                                        <td>${item._10月價格}</td>
                                        <td>${item._11月價格}</td>
                                        <td>${item._12月價格}</td>
                                    </tr>
                                `;
                    }
                }

                //將所選作物的資料添加到表格的 tbody 中
                $('#crop tbody').append(dataItem);
            }
        });
    });

    //當選擇年份發生變化時，執行以下程式碼
    $('#yearSelection').change(function () {
        let selectedYear = $(this).val(); //取得所選的年份
        let selectedCrop = $('#cropSelection').val(); //取得所選的作物
        $('#crop tbody').empty(); //清空表格

        //再次篩選資料，僅顯示所選年份和作物的資料
        $.ajax({
            type: 'get',
            url: '/api/AgriculturalProductOriginPrices',
            success: (data) => {
                let dataItem = "";
                for (let item of data) {
                    if ((item.年份 === selectedYear || selectedYear === "") && (item.作物 === selectedCrop || selectedCrop === "")) {
                        dataItem += `
                                    <tr>
                                        <td>${item.作物}</td>
                                        <td>${item.年份}</td>
                                        <td>${item._1月價格}</td>
                                        <td>${item._2月價格}</td>
                                        <td>${item._3月價格}</td>
                                        <td>${item._4月價格}</td>
                                        <td>${item._5月價格}</td>
                                        <td>${item._6月價格}</td>
                                        <td>${item._7月價格}</td>
                                        <td>${item._8月價格}</td>
                                        <td>${item._9月價格}</td>
                                        <td>${item._10月價格}</td>
                                        <td>${item._11月價格}</td>
                                        <td>${item._12月價格}</td>
                                    </tr>
                                `;
                    }
                }

                //將所選年份和作物的資料添加到表格的 tbody 中
                $('#crop tbody').append(dataItem);
            }
        });
    });
});