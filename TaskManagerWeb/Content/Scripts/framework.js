/**
 * Created by dudj on 2015-09-28
 */
// 声明一个全局对象Namespace，用来注册命名空间
var Namespace = Namespace || {};
// 全局对象仅仅存在register函数，参数为名称空间全路径，如"Ywdsoft.tools"
Namespace.register = function (fullNS) {
    // 将命名空间切成N部分, 比如Ywdsoft、tools等
    var nsArray = fullNS.split('.');
    var sEval = "";
    var sNS = "";
    for (var i = 0; i < nsArray.length; i++) {
        if (i != 0) sNS += ".";
        sNS += nsArray[i];
        // 依次创建构造命名空间对象（假如不存在的话）的语句
        // 比如先创建Ywdsoft，然后创建tools，依次下去
        sEval += "if (typeof(" + sNS + ") == 'undefined') {" + sNS + " = {};}"
    }
    if (sEval != "") { eval(sEval); }
}

/*
*  编辑页面状态
*/
var WebState = {
    //新增
    "ADD": 1,
    //编辑
    "EDIT": 2,
    //只读
    "READ": 3
};

var WebStateCN = {
    //新增
    "1": "新增",
    //编辑
    "2": "修改",
    //只读
    "3": "查看"
};

//ajax请求全局设置
$.ajaxSetup({
    //异步请求
    async: true,
    //缓存设置
    cache: false
});

$(document).ajaxComplete(function (evt, request, settings) {
    var text = request.responseText;
    if (text) {
        try {
            //Unauthorized  登录超时或者无权限
            if (request.status == "401") {
                var json = $.parseJSON(text);
                if (json.Message == "logout") {
                    //登录超时,弹出系统登录框
                    layer.open({
                        type: 2,
                        title: '登录',
                        shadeClose: true,
                        shade: 0.8,
                        area: ['320px', '220px'],
                        content: "/Login/LockScreen"
                    });
                } else {
                    layer.alert(json.ExceptionMessage ? json.ExceptionMessage : "系统异常，请联系系统管理员", {
                        title: "错误提醒",
                        icon: 2
                    });
                }
            } else if (request.status == "500") {
                var json = $.parseJSON(text);
                if (json.ShowException) {
                    $.ajax({
                        type: "post",
                        url: "/Error/Exception",
                        data: mvcParamMatch(json),
                        data: json,
                        dataType: "html",
                        success: function (data) {
                            //页面层
                            layer.open({
                                title: '异常信息',
                                type: 1,
                                shade: 0.8,
                                shift: -1,
                                area: ['100%', '100%'],
                                content: data,
                            });
                        }
                    });

                } else {
                    layer.alert("系统异常，请联系系统管理员", {
                        title: "错误提醒",
                        icon: 2
                    });
                }
            }
        } catch (e) {
            console.log(e);
        }
    }
});

var mvcParamMatch = (function () {
    var MvcParameterAdaptive = {};
    //验证是否为数组 
    MvcParameterAdaptive.isArray = Function.isArray || function (o) {
        return typeof o === "object" &&
        Object.prototype.toString.call(o) === "[object Array]";
    };
    //将数组转换为对象 
    MvcParameterAdaptive.convertArrayToObject = function (/*数组名*/arrName, /*待转换的数组*/array, /*转换后存放的对象，不用输入*/saveOjb) {
        var obj = saveOjb || {};
        function func(name, arr) {
            for (var i in arr) {
                if (!MvcParameterAdaptive.isArray(arr[i]) && typeof arr[i] === "object") {
                    for (var j in arr[i]) {
                        if (MvcParameterAdaptive.isArray(arr[i][j])) {
                            func(name + "[" + i + "]." + j, arr[i][j]);
                        } else if (typeof arr[i][j] === "object") {
                            MvcParameterAdaptive.convertObject(name + "[" + i + "]." + j + ".", arr[i][j], obj);
                        } else {
                            obj[name + "[" + i + "]." + j] = arr[i][j];
                        }
                    }
                } else {
                    obj[name + "[" + i + "]"] = arr[i];
                }
            }
        }
        func(arrName, array);
        return obj;
    };
    //转换对象 
    MvcParameterAdaptive.convertObject = function (/*对象名*/objName,/*待转换的对象*/turnObj, /*转换后存放的对象，不用输入*/saveOjb) {
        var obj = saveOjb || {};
        function func(name, tobj) {
            for (var i in tobj) {
                if (MvcParameterAdaptive.isArray(tobj[i])) {
                    MvcParameterAdaptive.convertArrayToObject(i, tobj[i], obj);
                } else if (typeof tobj[i] === "object") {
                    func(name + i + ".", tobj[i]);
                } else {
                    obj[name + i] = tobj[i];
                }
            }
        }
        func(objName, turnObj);
        return obj;
    };
    return function (json, arrName) {
        arrName = arrName || "";
        if (typeof json !== "object") throw new Error("请传入json对象");
        if (MvcParameterAdaptive.isArray(json) && !arrName) throw new Error("请指定数组名，对应Action中数组参数名称！");
        if (MvcParameterAdaptive.isArray(json)) {
            return MvcParameterAdaptive.convertArrayToObject(arrName, json);
        }
        return MvcParameterAdaptive.convertObject("", json);
    };
})();

/*
* 功能：    编辑界面保存以后改变模式和主键的值
* 参数：    无
* 返回值：  无
* 创建人：  杜冬军
* 创建时间：2013-12-22
*/
function setMode(id) {
    var mode = WebState.ADD;
    if (id) {
        mode = WebState.EDIT;
    }
    var url = setUrlParam("mode", mode, window.location.href);
    url = setUrlParam("id", id, url);
    window.location.href = url;
}

//关闭弹出层
function closeWindow(data) {
    var index = parent.layer.getFrameIndex(window.name);
    parent.layer.data = data;
    parent.layer.close(index);
}

/*
* 功能：    保存以后重新打开页面
* 参数：    无
* 返回值：  无
* 创建人：  杜冬军
* 创建时间：2013-12-22
*/
function setModeNew() {
    var mode = WebState.ADD;
    var url = setUrlParam("mode", mode, window.location.href);
    url = setUrlParam("id", "", url);
    window.location.href = url;
}

/*
* 功能：    只读页面跳转到编辑界面
* 参数：    无
* 返回值：  无
* 创建人：  杜冬军
* 创建时间：2016-03-02
*/
function changeRead2Edit() {
    window.location.href = setUrlParam("mode", WebState.EDIT, window.location.href);
}

//para_name 参数名称 para_value 参数值 url所要更改参数的网址
function setUrlParam(para_name, para_value, url) {
    var strNewUrl = new String();
    var strUrl = url;
    if (strUrl.indexOf("?") != -1) {
        strUrl = strUrl.substr(strUrl.indexOf("?") + 1);
        if (strUrl.toLowerCase().indexOf(para_name.toLowerCase()) == -1) {
            strNewUrl = url + "&" + para_name + "=" + para_value;
            return strNewUrl;
        } else {
            var aParam = strUrl.split("&");
            for (var i = 0; i < aParam.length; i++) {
                if (aParam[i].substr(0, aParam[i].indexOf("=")).toLowerCase() == para_name.toLowerCase()) {
                    aParam[i] = aParam[i].substr(0, aParam[i].indexOf("=")) + "=" + para_value;
                }
            }
            strNewUrl = url.substr(0, url.indexOf("?") + 1) + aParam.join("&");
            return strNewUrl;
        }
    } else {
        strUrl += "?" + para_name + "=" + para_value;
        return strUrl
    }
}

Namespace.register("Ywdsoft.Pub");
Ywdsoft.Pub = (function () {
    /*
    *  选择界面返回状态
    */
    var _SelectState = {
        //确定
        "OK": "OK",
        //取消
        "CANCEL": "Cancel",
        //清空
        "CLEAR": "Clear"
    };
    return {
        SelectState: _SelectState,
        /*
        * 功能：    选择界面取消按钮
        * 参数：    无
        * 返回值：  无
        * 创建人：  杜冬军
        * 创建时间：2015-10-14
        */
        CancelClick: function () {
            closeWindow({ Type: _SelectState.CANCEL });
        },
        /*
        * 功能：    选择界面确定按钮
        * 参数：    无
        * 返回值：  无
        * 创建人：  杜冬军
        * 创建时间：2015-10-14
        */
        OkClick: function (data) {
            closeWindow({ Type: _SelectState.OK, Data: data });
        },
        /*
        * 功能：     选择界面清空按钮
        * 参数：    无
        * 返回值：  无
        * 创建人：  杜冬军
        * 创建时间：2013-12-31
        */
        ClearClick: function (data) {
            closeWindow({ Type: _SelectState.CLEAR, Data: data });
        }
    }
})();


/***************************************************** 控件拓展   ********************************************************************************/
Namespace.register("Ywdsoft");
var Ywdsoft = (function () {

    var DAY_MS = 86400000,
	HOUR_MS = 3600000,
	MINUTE_MS = 60000;

    var JqGrid_settings = {
        datatype: "json",
        autowidth: true,
        rownumbers: false,
        styleUI: "Bootstrap",
        height: 'auto',
        rowNum: 20,
        rowList: [20, 50, 100, 200],
        rownumWidth: 40,
        loadtext: "努力为您加载中...",
        loadui: "block",
        multiselect: false,//是否多选
        mtype: 'post',
        pagerpos: "left",
        viewrecords: true,
        hidegrid: false,
        prmNames: { page: "page", rows: "pagesize", sort: "sortfield", order: "sortorder", search: null, nd: null, npage: null },
        jsonReader: {
            repeatitems: false,
            root: "Result",
            total: "TotalPage",
            records: "TotalCount"
        },
        emptyrecords: "",
        loadComplete: function (xhr) {
            var rowNum = parseInt($(this).getGridParam("records"), 10);
            var id = "#gbox_{0}".format(this.id);
            var nodataid = "{0}_datagridList_tip".format(this.id);
            var nodata = $("#" + nodataid);
            if (rowNum <= 0) {
                if (nodata.length == 0) {

                    nodata = $('<div id="' + nodataid + '" class="emptyTip4" style="position: absolute; height:100%; width: 100%; z-index: 100;"></div>');
                    $(id).prepend(nodata);
                }
                nodata.show();
            } else {
                nodata.hide();
            }
            var jsonReader = $(this).getGridParam("jsonReader");
            if (jsonReader && jsonReader.root) {
                //将获取到的原始数据缓存到对象上
                $("#" + this.id).data("srcJson", xhr[jsonReader.root]);
            }
        }
    };

    return {
        dateInfo: {
            monthsLong: ["一月", "二月", "三月", "四月", "五月", "六月", "七月", "八月", "九月", "十月", "十一月", "十二月"],
            monthsShort: ["1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月"],
            daysLong: ["星期日", "星期一", "星期二", "星期三", "星期四", "星期五", "星期六"],
            daysShort: ["日", "一", "二", "三", "四", "五", "六"],
            quarterLong: ['一季度', '二季度', '三季度', '四季度'],
            quarterShort: ['Q1', 'Q2', 'Q2', 'Q4'],
            halfYearLong: ['上半年', '下半年'],
            patterns: {
                "d": "yyyy-M-d",
                "D": "yyyy年M月d日",
                "f": "yyyy年M月d日 H:mm",
                "F": "yyyy年M月d日 H:mm:ss",
                "g": "yyyy-M-d H:mm",
                "G": "yyyy-M-d H:mm:ss",
                "m": "MMMd日",
                "o": "yyyy-MM-ddTHH:mm:ss.fff",
                "s": "yyyy-MM-ddTHH:mm:ss",
                "t": "H:mm",
                "T": "H:mm:ss",
                "U": "yyyy年M月d日 HH:mm:ss",
                "y": "yyyy年MM月"
            },
            tt: {
                "AM": "上午",
                "PM": "下午"
            },
            ten: {
                "Early": "上旬",
                "Mid": "中旬",
                "Late": "下旬"
            },
            today: '今天',
            clockType: 24
        },
        fixDate: function (d, check) {
            if (+d) {
                while (d.getDate() != check.getDate()) {
                    d.setTime(+d + (d < check ? 1 : -1) * HOUR_MS);
                }
            }
        },
        parseISO8601: function (s, ignoreTimezone) {
            var m = s.match(/^([0-9]{4})([-\/]([0-9]{1,2})([-\/]([0-9]{1,2})([T ]([0-9]{1,2}):([0-9]{1,2})(:([0-9]{1,2})(\.([0-9]+))?)?(Z|(([-+])([0-9]{2})(:?([0-9]{2}))?))?)?)?)?$/);
            if (!m) {

                m = s.match(/^([0-9]{4})[-\/]([0-9]{2})[-\/]([0-9]{2})[T ]([0-9]{1,2})/);
                if (m) {
                    var date = new Date(m[1], m[2] - 1, m[3], m[4]);
                    return date;
                }


                m = s.match(/^([0-9]{4}).([0-9]*)/);
                if (m) {
                    var date = new Date(m[1], m[2] - 1);
                    return date;
                }


                m = s.match(/^([0-9]{4}).([0-9]*).([0-9]*)/);
                if (m) {
                    var date = new Date(m[1], m[2] - 1, m[3]);
                    return date;
                }


                m = s.match(/^([0-9]{2})-([0-9]{2})-([0-9]{4})$/);
                if (!m) return null;
                else {
                    var date = new Date(m[3], m[1] - 1, m[2]);
                    return date;
                }
            }
            var date = new Date(m[1], 0, 1);
            if (ignoreTimezone || !m[14]) {
                var check = new Date(m[1], 0, 1, 9, 0);
                if (m[3]) {
                    date.setMonth(m[3] - 1);
                    check.setMonth(m[3] - 1);
                }
                if (m[5]) {
                    date.setDate(m[5]);
                    check.setDate(m[5]);
                }
                Ywdsoft.fixDate(date, check);
                if (m[7]) {
                    date.setHours(m[7]);
                }
                if (m[8]) {
                    date.setMinutes(m[8]);
                }
                if (m[10]) {
                    date.setSeconds(m[10]);
                }
                if (m[12]) {
                    date.setMilliseconds(Number("0." + m[12]) * 1000);
                }
                Ywdsoft.fixDate(date, check);
            } else {
                date.setUTCFullYear(
                    m[1],
                    m[3] ? m[3] - 1 : 0,
                    m[5] || 1
                );
                date.setUTCHours(
                    m[7] || 0,
                    m[8] || 0,
                    m[10] || 0,
                    m[12] ? Number("0." + m[12]) * 1000 : 0
                );
                var offset = Number(m[16]) * 60 + (m[18] ? Number(m[18]) : 0);
                offset *= m[15] == '-' ? 1 : -1;
                date = new Date(+date + (offset * 60 * 1000));
            }
            return date;
        },
        /*
          * 功能： 把字符串转换成Date类型对象
          * 参数：
                  支持如下字符串转换成日期：
                  2010-11-22
                  2010/11/22
                  11-22-2010
                  11/22/2010
                  2010-11-22T23:23:59
                  2010/11/22T23:23:59
                  2010-11-22 23:23:59
                  2010/11/22 23:23:59
          */
        parseDate: function (s, ignoreTimezone) {
            try {
                var d = eval(s);
                if (d && d.getFullYear) return d;
            } catch (ex) {
            }

            if (typeof s == 'object') {
                return isNaN(s) ? null : s;
            }
            if (typeof s == 'number') {

                var d = new Date(s * 1000);
                if (d.getTime() != s) return null;
                return isNaN(d) ? null : d;
            }
            if (typeof s == 'string') {

                m = s.match(/^([0-9]{4})([0-9]{2})([0-9]{2})$/);
                if (m) {
                    var date = new Date(m[1], m[2] - 1, m[3]);
                    return date;
                }


                m = s.match(/^([0-9]{4}).([0-9]*)$/);
                if (m) {
                    var date = new Date(m[1], m[2] - 1);
                    return date;
                }

                if (s.match(/^\d+(\.\d+)?$/)) {
                    var d = new Date(parseFloat(s) * 1000);
                    if (d.getTime() != s) return null;
                    else return d;
                }
                if (ignoreTimezone === undefined) {
                    ignoreTimezone = true;
                }
                var d = Ywdsoft.parseISO8601(s, ignoreTimezone) || (s ? new Date(s) : null);
                return isNaN(d) ? null : d;
            }

            return null;
        },
        /*
        * 功能：    日期格式化功能
        * 参数：    format：格式化
        * 参数取值说明 format
            d	月中的某一天。一位数的日期没有前导零。
            dd	月中的某一天。一位数的日期有一个前导零。
            ddd	周中某天的缩写名称
            dddd	周中某天的完整名称
            M	月份数字。一位数的月份没有前导零。
            MM	月份数字。一位数的月份有一个前导零。
            MMM	月份的缩写名称。
            MMMM	月份的完整名称。
            y	不包含纪元的年份。如果不包含纪元的年份小于 10，则显示不具有前导零的年份。
            yy	不包含纪元的年份。如果不包含纪元的年份小于 10，则显示具有前导零的年份。
            yyyy	包括纪元的四位数的年份。
            h	12 小时制的小时。一位数的小时数没有前导零。
            hh	12 小时制的小时。一位数的小时数有前导零。
            H	24 小时制的小时。一位数的小时数没有前导零。
            HH	24 小时制的小时。一位数的小时数有前导零。
            m	分钟。一位数的分钟数没有前导零。
            mm	分钟。一位数的分钟数有一个前导零。
            s	秒。一位数的秒数没有前导零。
            ss	秒。一位数的秒数有一个前导零。
        * 创建人：  杜冬军
        * 创建时间：2015-11-18
        */
        formatDate: function (date, format) {
            if (!date || !date.getFullYear || isNaN(date)) return "";
            var fd = date.toString();

            var dateFormat = Ywdsoft.dateInfo;
            if (!dateFormat) dateFormat = Ywdsoft.dateInfo;

            if (typeof (dateFormat) !== "undefined") {
                var pattern = typeof (dateFormat.patterns[format]) !== "undefined" ? dateFormat.patterns[format] : format;

                var year = date.getFullYear();
                var month = date.getMonth();
                var day = date.getDate();

                if (format == "yyyy-MM-dd") {
                    month = month + 1 < 10 ? "0" + (month + 1) : month + 1;
                    day = day < 10 ? "0" + day : day;
                    return year + "-" + month + "-" + day;
                }
                if (format == "MM/dd/yyyy") {
                    month = month + 1 < 10 ? "0" + (month + 1) : month + 1;
                    day = day < 10 ? "0" + day : day;
                    return month + "/" + day + "/" + year;
                }



                fd = pattern.replace(/yyyy/g, year);
                fd = fd.replace(/yy/g, (year + "").substring(2));


                var halfyear = date.getHalfYear();
                fd = fd.replace(/hy/g, dateFormat.halfYearLong[halfyear]);


                var quarter = date.getQuarter();
                fd = fd.replace(/Q/g, dateFormat.quarterLong[quarter]);
                fd = fd.replace(/q/g, dateFormat.quarterShort[quarter]);


                fd = fd.replace(/MMMM/g, dateFormat.monthsLong[month].escapeDateTimeTokens());
                fd = fd.replace(/MMM/g, dateFormat.monthsShort[month].escapeDateTimeTokens());
                fd = fd.replace(/MM/g, month + 1 < 10 ? "0" + (month + 1) : month + 1);
                fd = fd.replace(/(\\)?M/g, function ($0, $1) { return $1 ? $0 : month + 1; });

                var dayOfWeek = date.getDay();
                fd = fd.replace(/dddd/g, dateFormat.daysLong[dayOfWeek].escapeDateTimeTokens());
                fd = fd.replace(/ddd/g, dateFormat.daysShort[dayOfWeek].escapeDateTimeTokens());


                fd = fd.replace(/dd/g, day < 10 ? "0" + day : day);
                fd = fd.replace(/(\\)?d/g, function ($0, $1) { return $1 ? $0 : day; });

                var hour = date.getHours();
                var halfHour = hour > 12 ? hour - 12 : hour;
                if (dateFormat.clockType == 12) {
                    if (hour > 12) {
                        hour -= 12;
                    }
                }


                fd = fd.replace(/HH/g, hour < 10 ? "0" + hour : hour);
                fd = fd.replace(/(\\)?H/g, function ($0, $1) { return $1 ? $0 : hour; });


                fd = fd.replace(/hh/g, halfHour < 10 ? "0" + halfHour : halfHour);
                fd = fd.replace(/(\\)?h/g, function ($0, $1) { return $1 ? $0 : halfHour; });

                var minutes = date.getMinutes();
                fd = fd.replace(/mm/g, minutes < 10 ? "0" + minutes : minutes);
                fd = fd.replace(/(\\)?m/g, function ($0, $1) { return $1 ? $0 : minutes; });

                var seconds = date.getSeconds();
                fd = fd.replace(/ss/g, seconds < 10 ? "0" + seconds : seconds);
                fd = fd.replace(/(\\)?s/g, function ($0, $1) { return $1 ? $0 : seconds; });

                fd = fd.replace(/fff/g, date.getMilliseconds());

                fd = fd.replace(/tt/g, date.getHours() > 12 || date.getHours() == 0 ? dateFormat.tt["PM"] : dateFormat.tt["AM"]);


                var date = date.getDate();
                var tenF = '';
                if (date <= 10) tenF = dateFormat.ten['Early'];
                else if (date <= 20) tenF = dateFormat.ten['Mid'];
                else tenF = dateFormat.ten['Late'];
                fd = fd.replace(/ten/g, tenF);
            }

            return fd.replace(/\\/g, "");
        },
        /*
        * 功能：  克隆对象
        * 参数：  obj 待克隆对象
        * 创建人：  杜冬军
        * 创建时间：2015-12-24
        */
        clone: function (obj) {
            return $.extend(false, null, obj);
        },
        getDataOptions: function (ctl) {
            //初始化日期
            var options = $(ctl).attr("data-options");
            var optJson = {};
            if (options) {
                optJson = JSON.parse(options);
            }
            //默认为通过HTML方式初始化控件
            if (optJson.isInit == undefined) {
                optJson.isInit = true;
            }
            return optJson;
        },
        /*
        * 功能：  将laydate需要的格式化字符串转换成系统内置的
        * 创建人：  杜冬军
        * 创建时间：2015-12-11
        */
        parseDateFormat: function (format) {
            if (format) {
                return format.replace("YYYY", "yyyy").replace("DD", "dd").replace("hh", "HH");
            }
            return null;
        },
        /*
        * 功能：  界面初始化时,解析所有自定义封装控件
        * 创建人：  杜冬军
        * 创建时间：2015-12-11
        */
        parse: function () {
            var options;
            //解析表单控件
            $(".form-control").each(function (i, ctl) {
                switch (ctl.nodeName) {
                    case "INPUT":
                        //日期控件
                        if ($(ctl).hasClass("layer-date")) {
                            options = Ywdsoft.getDataOptions(ctl);
                            if (options.isInit) {
                                $(ctl).dateTime(options);
                            }
                        }
                        else if ($(ctl).hasClass("Ywdsoft-PlateNo")) {
                            //初始化车牌选择
                            //获取初始化选项
                            options = Ywdsoft.getDataOptions(ctl);
                            $(ctl).plateNoTools(options);
                        }
                        break;
                    case "SELECT":
                        //解析SELECT控件
                        $(ctl).combobox();
                        break;
                    default:
                        break;
                }
            });

            //解析radio
            var arrRadio = $('.i-checks');
            if (arrRadio.length > 0 && typeof (arrRadio.iCheck) == "function") {
                arrRadio.iCheck({
                    checkboxClass: 'icheckbox_square-green',
                    radioClass: 'iradio_square-green',
                });
            }

            //caclHeight();
            //if ($(".mini-fit").length > 0) {
            //    $(window).on("resize", function () {
            //        caclHeight();
            //    });
            //}

            //function caclHeight() {
            //    //解析mini-fit自适应
            //    var totalHeight = 0;
            //    $(".mini-fit").siblings().each(function (i, item) {
            //        totalHeight += $(item).height();
            //    });
            //    $(".mini-fit").height($(window).height() - totalHeight - 100);
            //}
        },
        /*
        * 功能：  表单控件封装
        * 参数:   id:表单ID
        *         mode:表单模式
        * 创建人：  杜冬军
        * 创建时间：2015-12-15
        */
        Form: function (id, mode) {
            var _isChange = false;

            var formCtl = $("#" + id);

            //默认参数
            var defaults = { isTrim: false };

            //表单赋值
            this.setData = function (data) {
                var options, value;
                if (mode == WebState.READ) {
                    $(formCtl.find('.readSpan')).each(function (i, item) {
                        var name = $(item).attr("name");
                        if (data[name] == undefined) return;
                        options = Ywdsoft.getDataOptions(item);
                        value = data[name];
                        if (options.format) {
                            if (options.format.indexOf("YYYY") == 0) {
                                value = data[name].formatDateString(Ywdsoft.parseDateFormat(options.format));
                            }
                        }
                        $(item).text(value);
                    });
                } else {
                    return formCtl.each(function () {
                        var input, name;
                        if (data == null) { this.reset(); return; }
                        for (var i = 0; i < this.length; i++) {
                            input = this.elements[i];
                            //checkbox的name可能是name[]数组形式
                            name = (input.type == "checkbox") ? input.name.replace(/(.+)\[\]$/, "$1") : input.name;
                            if (data[name] == undefined) continue;
                            switch (input.type) {
                                case "checkbox":
                                    if (data[name] == "") {
                                        input.checked = false;
                                    } else {
                                        //数组查找元素
                                        if (data[name].indexOf(input.value) > -1) {
                                            input.checked = true;
                                        } else {
                                            input.checked = false;
                                        }
                                    }
                                    break;
                                case "radio":
                                    if ($(input).parent().hasClass('iradio_square-green')) {
                                        if (data[name].toString() == "") {
                                            $(input).iCheck('uncheck');
                                        } else if (input.value == data[name].toString()) {
                                            $(input).iCheck('check');
                                        }
                                    } else {
                                        if (data[name].toString() == "") {
                                            input.checked = false;
                                        } else if (input.value == data[name].toString()) {
                                            input.checked = true;
                                        }
                                    }
                                    break;
                                case "button": break;
                                case "text":
                                    if ($(input).hasClass("layer-date") || $(input).hasClass("formatDate")) {
                                        options = Ywdsoft.getDataOptions(input);
                                        if (options.format) {
                                            input.value = data[name].formatDateString(Ywdsoft.parseDateFormat(options.format));
                                        } else {
                                            input.value = data[name];
                                        }
                                    } else {
                                        input.value = data[name];
                                    }
                                    break;
                                default: input.value = data[name];
                            }
                        }
                    });
                }
            }

            //表单获取数据
            //参数isTrim 表示是否去掉字符串两边空格
            this.getData = function (options) {
                var settings = $.extend(false, defaults, options);
                var json = {}, value;
                $(formCtl.serializeArray()).each(function (i, item) {
                    value = settings.isTrim && typeof (item.value) == "string" ? item.value.trim() : item.value;
                    if (json.hasOwnProperty(item.name)) {
                        var arrValue = json[item.name];
                        if (!$.isArray(arrValue)) {
                            json[item.name] = [arrValue];
                        }
                        json[item.name].push(value);
                    } else {
                        json[item.name] = value;
                    }
                });
                //处理界面特殊元素
                for (var key in json) {
                    var arrValue = json[key];
                    if (!$.isArray(arrValue)) {
                        //多选插件multiple
                        if ($("select[multiple][name='{0}']".format(key)).length > 0) {
                            // 根据前面的处理逻辑，如果已经多选，则在上面已经将多选封装成数据，无需再次封装
                            // by chenf 2016-01-02 12:14
                            json[key] = [arrValue];
                        }
                    }
                }
                return json;
            }
            //是否导入了表单验证
            var isImportValidate = $.isFunction(formCtl.validate);
            if (isImportValidate) {
                formCtl.validate({
                    debug: true,
                    //验证插件,排除没有name属性的元素
                    ignore: ":not([name])",
                    errorClass: 'help-block',
                    highlight: function (element) {
                        $(element).closest('div').addClass('has-error');
                    },

                    success: function (label) {
                        label.closest('div').removeClass('has-error');
                        label.remove();
                    },

                    errorPlacement: function (error, element) {
                        var text = $(error).text().trim();
                        $(error).attr("title", text);
                        element.parent().append(error);
                    }
                });
            }
            //表单校验
            this.validate = function () {
                if (isImportValidate) {
                    return formCtl.valid();
                } else {
                    return false;
                }
            }

            //表单重置
            this.reset = function () {
                formCtl.get(0).reset();
            }

            //判断是否变动
            this.isChanged = function () {
                return _isChange;
            }

            this.setChanged = function (isChange) {
                this._isChange = isChange;
            }

            //禁用页面所有按钮和输入框
            this.setDisabled = function (bDisabled) {
                if (bDisabled) {
                    formCtl.find("input,button,select,textarea").each(function (i, item) {
                        if (!$(item).hasClass("detail-mode")) {
                            $(item).attr("disabled", true).addClass("form-control-disabled");
                        }
                    });
                    formCtl.find(".editing-mode").each(function (i, item) {
                        $(item).hide();
                    });
                }
            }

            //表单初始化数据
            //var _initData = this.getData();
            var _initData = {};

        },
        //列表控件
        Grid: function (id, options) {
            var gridCtl = $("#" + id);
            if (gridCtl.length == 0) {
                throw Error("未找到id:{0}的控件".format(id));
            }
            var parentNode = gridCtl.parent();

            if (!options || !options.idField) {
                throw Error("必须设置表格的idField对应字段");
            }
            var settings = $.extend(false, JqGrid_settings, options);

            //设置数据主键
            settings.jsonReader.id = settings.idField;
            if (!settings.pager && settings.isPager != false) {
                //设置分页控件
                settings.pager = "#{0}_Pager".format(id);
                gridCtl.after("<div id='{0}_Pager'></div>".format(id));
            }

            $(settings.colModel).each(function (i, item) {
                if (!item.index) {
                    return;
                }
                if (!item.name) {
                    item.name = item.index;
                }
            });

            gridCtl.jqGrid(settings);

            if (settings.ShowRefresh == undefined) {
                settings.ShowRefresh = true;
            }
            //添加表单刷新按钮
            if (settings.ShowRefresh) {
                gridCtl.navGrid(settings.pager, {
                    edit: false,
                    add: false,
                    del: false,
                    search: false,
                    position: "center"
                });
            }


            if (parentNode.length > 0) {
                gridCtl.setGridWidth(parentNode.width());
                //if (isNaN(settings.height) && settings.height != "auto") {
                //    gridCtl.setGridHeight(parentNode.height());
                //}
                $(window).on("resize", function () {
                    gridCtl.setGridWidth(parentNode.width());
                });
            }

            //参数化查询列表
            this.load = function (paramList) {
                gridCtl.jqGrid('setGridParam', { postData: [] });
                gridCtl.jqGrid('setGridParam', { postData: paramList, page: 1 }).trigger("reloadGrid");
            }

            //重新刷新列表
            this.reload = function () {
                gridCtl.trigger("reloadGrid");
            }

            this.getColumnInfo = function () {
                var columnInfos = new Array();
                var colNames = gridCtl.jqGrid('getGridParam', 'colNames');
                var colModel = gridCtl.jqGrid('getGridParam', 'colModel');

                $.each(colModel, function (index, item) {
                    //收集有绑定列 并且 为可见的 的数据
                    if ((!item.hidden || (item.hidden && item.export)) && item.name != "rn" && item.name != "cb" && (item.name || item.index)) {
                        columnInfos.push({
                            "Align": item.align ? item.align : "left",
                            "Header": colNames[index],
                            "Field": item.name ? item.name : item.index
                        });
                    }
                });
                return columnInfos;

            }

            //获取查询参数对象
            this.getLoadParams = function () {
                return gridCtl.jqGrid("getGridParam", "postData");
            }

            //获取当前分页查询参数
            this.getPagerParams = function () {
                var pager = gridCtl.getGridParam('prmNames');

                var page = gridCtl.getGridParam('page'); // 当前页码
                var rows = gridCtl.getGridParam('rowNum'); // 每页大小  
                var sidx = gridCtl.getGridParam('sortorder'); // 排序方向
                var sord = gridCtl.getGridParam('sortname'); // 排序字段

                var result = {};
                result[pager.page] = page;
                result[pager.rows] = rows;
                result[pager.sort] = sord;
                result[pager.order] = sidx;
                return result;
            }

            //获取当前选中行
            this.getSelected = function () {
                var rowid = gridCtl.jqGrid('getGridParam', 'selrow');
                if (!rowid) {
                    return null;
                }
                return _getRowData(rowid);
            }

            //获取主键获取行数据
            this.getRowData = function (rowid) {
                if (!rowid) {
                    return null;
                }
                return _getRowData(rowid);
            }

            function _getRowData(rowid) {
                var row = gridCtl.getRowData(rowid);
                row[settings.idField] = rowid;
                return row;
            }

            //获取所有选中的行
            this.getSelecteds = function () {
                var rowids = gridCtl.jqGrid('getGridParam', 'selarrrow');
                if (rowids && rowids.length > 0) {
                    var arrRow = new Array();
                    for (var i = 0; i < rowids.length; i++) {
                        arrRow.push(_getRowData(rowids[i]));
                    }
                    return arrRow;
                } else {
                    return [];
                }
            }

            //获取所有选中的行原始数据
            this.getSelectedRows = function () {
                var rowids = gridCtl.jqGrid('getGridParam', 'selarrrow');
                if (rowids && rowids.length > 0) {
                    var srcData = gridCtl.data("srcJson");
                    var o = [], item, key;
                    for (var i = 0, length = srcData.length; i < length; i++) {
                        item = srcData[i];
                        key = item[settings.idField];
                        if (!key) {
                            key = i + 1;
                        }
                        o[key] = item;
                    }
                    var arrRow = new Array();
                    for (var i = 0; i < rowids.length; i++) {
                        arrRow.push(o[rowids[i]]);
                    }
                    return arrRow;
                } else {
                    return [];
                }
            }

            //隐藏列
            this.hideColumn = function (column) {
                if (this.isShowColumn(column)) {
                    gridCtl.hideCol(column);
                    gridCtl.setGridWidth(parentNode.width());
                }
            }

            //显示列
            this.showColumn = function (column) {
                if (!this.isShowColumn(column)) {
                    gridCtl.showCol(column);
                    gridCtl.setGridWidth(parentNode.width());
                }
            }

            //列是否显示
            this.isShowColumn = function (column) {
                var colModel = gridCtl.jqGrid('getGridParam', 'colModel');
                var isShow = false;
                $(colModel).each(function (i, item) {
                    if (item.name == column) {
                        isShow = !item.hidden;
                        return false;
                    }
                });
                return isShow;
            }

            /*
            * 功能：    标准Gird导出数据到EXECL
            * 参数：    fileName:导出EXECL文件名
            *           data:需要导出的数据  如果为空则从数据库查询所有符合条件的数据最多20000条
            * 返回值：  无
            * Example:  grid.Export("日志列表")
            * 创建人：  杜冬军
            * 创建时间：2015-12-16
            */
            this.Export = function (fileName, data) {
                var columnInfos = this.getColumnInfo();
                var type = settings.mtype;

                if (columnInfos && columnInfos.length > 0) {
                    var options = {
                        columnInfos: columnInfos,
                        fileName: fileName,
                        type: type
                    };

                    //默认不分页导出数据
                    if (!$.isArray(data)) {
                        options.condition = this.getLoadParams();
                        options.api = settings.url;
                    } else {
                        options.data = data;
                    }
                    $.ExportXLS(options);
                }
            }

            this.setGridParam = function (paramName, data) {
                gridCtl.setGridParam({ paramName: data });
            }

            //重新加载本地数据
            this.reloadLocalData = function (data) {
                gridCtl.clearGridData();
                gridCtl.setGridParam({ "data": data });
                gridCtl.trigger("reloadGrid");
            }

            this.on = function (event, callback) {
                if (event && typeof (callback) == "function") {
                    switch (event.toLowerCase()) {
                        case "rowdblclick":
                            gridCtl.setGridParam({ 'ondblClickRow': callback });
                            break;
                        case "drawcell":
                            break;
                        default:
                            var key = 'on' + event;
                            gridCtl.setGridParam({ key: callback });
                            break;
                    }
                }
            }
        },

        delConfirm: function (callback, tip) {
            tip = tip || '确定删除记录？';
            layer.confirm(tip, {
                btn: ['确定', '取消'] //按钮
            }, function (index) {
                layer.close(index);
                if (typeof (callback) == "function") {
                    callback();
                }
            });
        }
    };
})();

(function ($) {
    var arrId = [];
    var arrType = { "datetime": "Ywdsoft_dateTime" };
    $.fn.extend({
        YwdsoftId: function (type) {
            var id = $(this).attr("id");
            if (!id) {
                id = "{0}_{1}".format(type, arrId.length + 1);
                arrId.push(id);
                $(this).attr("id", id);
            }
            return this;
        },
        /*
        * 功能：  combobox控件
        * 创建人：  杜冬军
        * 创建时间：2015-12-11
        * 功能：添加从后台取数功能
        */
        combobox: function (options) {
            var that = this;
            var url = $(that).attr("data-url");
            var textField = $(that).attr("textField");
            var valueField = $(that).attr("valueField");
            var showNullItem = $(that).attr("showNullItem");
            var callback = $(that).attr("callback");
            var type = $(that).attr("data-type");
            var async = $(that).attr("data-async");
            type = type || "get";
            if (async) {
                async = async.toBool();
            } else {
                async = true;
            }
            //动态生成option
            if (url && valueField) {
                if (!textField) {
                    textField = valueField;
                }
                $.ajax({
                    url: url,
                    type: type,
                    dataType: "json",
                    async: async,
                    success: function (json) {
                        var strHTML = "";
                        //判断是否含有空项
                        if (showNullItem) {
                            strHTML += "<option value=''></option>";
                        }
                        $(json).each(function (j, data) {
                            strHTML += "<option value='{0}'>{1}</option>".format(data[valueField], data[textField]);
                        });
                        $(that).html(strHTML);

                        if (callback) {
                            try {
                                eval("{0}({1})".format(callback, JSON.stringify(json)));
                            } catch (e) {
                                console.log(e);
                            }
                        }
                    }
                });
            }
            return this;
        },

        /*
        * 功能：  dateTime控件
        * 创建人：  杜冬军
        * 创建时间：2015-12-11
        */
        dateTime: function (options, callback) {
            var defaults = { "event": "click", "format": "YYYY-MM-DD" };
            var opt = $.extend(defaults, options);
            $(this).YwdsoftId(arrType.datetime);
            var id = $(this).attr("id");
            opt.elem = "#{0}".format(id);
            laydate(opt);
            return this;
        },
        /*
        * 功能：    查询框控件
        * 参数：    options：查询条件数据
        * 返回值：  无
        * 创建人：  杜冬军
        * 创建时间：2015-12-21
        * 修改日期：2016-01-11
        * 修改人  ：杜冬军
        * 修改内容：添加选项是否显示全部的功能 目前只有单选的时候该选项才起作用，后续进行优化
        * 版本修改记录
        * 2016-09-08 添加搜索框赋值方法 
        */
        search: function (options) {
            //展开收缩有回调事件时，默认最大展示条数
            var MAX_SHOW_COUNT = 10;
            //id前缀
            var ID_STUFF = "searchitem_";

            var searchCtl = this;
            var filterBtn = $('<div class="filter_btn"><span class="expand">展开条件</span></div>');

            var defaults = {
                //展开更多条件回调事件
                //参数：state  true表示展开  false 收缩
                "expandEvent": function (state) {
                },
                //默认展开条件数
                "expandRow": 2,
                //查询框
                "searchBoxs": [],
                //查询事件
                "search": function (paramList) {
                },
                //参数收集时返回值的Key
                "paramkey": "ValueList",
                //参数收集时自定义条件返回值的Key
                "paramCustomkey": "CustomList",
                //点击选项时是否立即进行查询 默认为true
                "searchOnSelect": true
            };

            //查询控件参数
            var settings = $.extend(defaults, options);

            //处理数据
            if (isNaN(settings.expandRow) || settings.expandRow < 1) {
                throw Error("默认展开条件数'expandRow'必须为大于0的整数");
            } else {
                if (settings.expandRow > settings.searchBoxs.length) {
                    settings.expandRow = settings.searchBoxs.length;
                }
            }
            //默认展开高度 每行高度40 - 下边框高度1
            var _expandHeight = settings.expandRow * 40 - (settings.expandRow - 1) * 1;
            searchCtl.css({ 'height': _expandHeight });

            $(settings.searchBoxs).each(function (i, item) {
                //1.id处理
                if (item.id && typeof (item.id) == "string") {
                    item.srcID = item.id;
                    item.id = "{0}{1}".format(ID_STUFF, item.id);
                } else {
                    item.srcID = i;
                    item.id = "{0}{1}".format(ID_STUFF, i);
                }


                //2.值域 文本域 绑定字段
                if (item.valueField || item.textField) {
                    if (item.valueField && !item.textField) {
                        item.textField = item.valueField;
                    }

                    if (item.textField && !item.valueField) {
                        item.valueField = item.textField;
                    }
                } else {
                    item.valueField = "value";
                    item.textField = "text";
                }

                //3.默认值处理
                if (!item.defaults) {
                    item.defaults = [];
                }

                item.selected = [];
                for (var j = 0; j < item.defaults.length; j++) {
                    item.selected.push(item.defaults[j]);
                }
                item.customSelectd = [];

                //4.是否多选处理,默认为单选
                if (item.isMultiple == undefined) {
                    item.isMultiple = false;
                }

                //5.是否显示全部选项
                if (item.isShowAll == undefined) {
                    item.isShowAll = true;
                }
            });


            //生成查询控件HTML
            _createCtrl();


            ////////////////////////////////////////事件绑定////////////////////////////////////////

            //给选项添加了自定义事件 select 对外调用
            searchCtl.on("click select", ".filter_option span", function (e) {
                var item = _getItem(this);
                var index = $(this).attr("data-id");
                //当前操作状态 select:当前元素选中 cancel:当前元素取消选中 cancelall:全部  selectremove:当前元素选中 其它元素移除选中 单选
                var state = "select";

                if (item.isMultiple == false || $(this).hasClass("option_all")) {
                    //单选
                    $(this).addClass("selected").siblings("span").removeClass("selected");

                    item.selected = [];
                    if (!$(this).hasClass("option_all")) {
                        item.selected.push(item.data[index][item.valueField]);
                        state = "selectremove";
                    } else {
                        state = "cancelall";
                    }
                } else {
                    //多选

                    //当前已经选中,则取消选中
                    if ($(this).hasClass("selected")) {
                        $(this).removeClass("selected");
                        state = "cancel";
                        //删除元素,寻找要删除的元素在数组的位置
                        var val = item.data[index][item.valueField];
                        for (var i = 0, length = item.selected.length; i < length; i++) {
                            if (item.selected[i] == val) {
                                item.selected.splice(i, 1);
                            }
                        }

                        if (item.selected.length == 0) {
                            $(this).siblings(".option_all").addClass("selected");
                            state = "cancelall";
                        }
                    } else {
                        $(this).addClass("selected");
                        $(this).siblings(".option_all").removeClass("selected");
                        item.selected.push(item.data[index][item.valueField]);
                        state = "select";
                    }
                }

                //清空自定义查询默认值
                _clearCustomValue(item);

                //选择项触发回调事件
                if (typeof (item.onSelect) == "function" && e.type == "click") {
                    item.onSelect(item.data[index], state);
                }

                //触发查询事件
                if (typeof (settings.search) == "function" && settings.searchOnSelect) {
                    settings.search(_getParamList());
                }
            })

            //收缩展开监听事件
            searchCtl.on("click", ".r", function (e, data) {
                _itemExpand(e, this, data);
            });

            //更多条件展开收缩
            filterBtn.find('span').on("click", function () {
                var state = true;
                if ($(this).hasClass('expand')) {
                    $(this).text('收缩条件').removeClass('expand');
                    searchCtl.css({ 'height': 'auto' });
                } else {
                    $(this).text('展开条件').addClass('expand');
                    searchCtl.css({ 'height': _expandHeight });
                    state = false;
                }
                if (typeof (settings.expandEvent) == "function") {
                    settings.expandEvent(state);
                }
            });

            //自定义条件确定按钮点击事件
            searchCtl.on("click", '.filter_custom .btn_filter_sure', function () {
                var index = $(this).attr("data-id");
                var item = settings.searchBoxs[index];

                var start = $("#{0}_c_custom_start".format(item.id)).val();

                var end;
                if (item.custom.isRange) {
                    end = $("#{0}_c_custom_end".format(item.id)).val();
                }

                //自定义条件搜索按钮点击触发回调事件,用于用于校验输入数据是否正确
                if (typeof (item.custom.event) == "function") {
                    if (!item.custom.event(start, end)) {
                        return;
                    }
                }
                //清空当前项其它选择条件
                $(this).closest(".filter_custom").siblings('.filter_option').find('span').removeClass('selected');
                item.selected = [];


                item.customSelectd[0] = start;

                if (item.custom.isRange) {
                    item.customSelectd[1] = end;
                }

                //触发查询事件
                if (typeof (settings.search) == "function") {
                    settings.search(_getParamList());
                }
            });

            ////////////////////////////////////////私有方法////////////////////////////////////////
            /*
            * 功能：    根据数据初始化查询控件
            * 参数：    当前项元素
            * 返回值：  当前项数据
            * 创建人：  杜冬军
            * 创建时间：2015-12-21
            */
            function _createCtrl() {
                var strHTML = "";

                $(settings.searchBoxs).each(function (i, item) {
                    strHTML += ('<div class="searchbox-item" {0} data-id="{1}" id="{2}">'.format((i + 1) == settings.searchBoxs.length ? 'style="border: 0"' : "", i, item.id) +
                        '<div class="l" id="{1}_l">{0}<i></i></div>'.format(item.title, item.id) +
                        '<div class="c" id="{0}_c">'.format(item.id) +
                            '<div class="control-type">({0})</div><div class="filter_option" style="padding-right:{1}px;">'.format(item.isMultiple ? "多选" : "单选", _getCustomDivWidth(item) + 20) + _createOptions(item) +
                            '</div>' + _createCustomFilter(i, item) +
                        '</div>' +
                        '<a href="javascript:;" class="r" id="{0}_r"><span class="text">展开</span><i class="fa fa-angle-double-down"></i></a>'.format(item.id) +
                    '</div>');
                });

                searchCtl.html(strHTML);
                $(".searchbox .searchbox-item").each(function (i) {
                    var height = $(this).find(".filter_option").outerHeight();
                    if (height <= 30) {
                        $(this).find(".r").remove();
                    }
                });
                //如果默认展开行数小于总条数
                if (settings.expandRow < settings.searchBoxs.length) {
                    searchCtl.after(filterBtn);
                }
            }

            //获取自定义查询框宽度
            function _getCustomDivWidth(item) {
                if (item.custom) {
                    if (item.custom.isRange == true) {
                        if (item.type == "date") {
                            return 320;
                            //为年月日类型
                        } else if (item.type == "datetime") {
                            return 440;
                        } else {
                            return 260;
                        }
                    } else {
                        if (item.type == "date") {
                            return 200;
                            //为年月日类型
                        } else if (item.type == "datetime") {
                            return 260;
                        } else {
                            return 170;
                        }
                    }
                } else {
                    return 0;
                }
            }

            //创建单个查询条件选项
            function _createOptions(item) {
                //创建全部
                var strHTML = "";
                if (item.isMultiple || (!item.isMultiple && item.isShowAll)) {
                    strHTML = '<span title="全部" class="option_all {0}">全部</span>'.format((!item.defaults || item.defaults.length == 0) ? "selected" : "");
                }
                //是否设置了回调事件,r如果设置了回调事件 ，则只输出前10项
                var isHasExpandCallBack = _isHasExpandEvent(item);
                var max = MAX_SHOW_COUNT;
                if (isHasExpandCallBack) {
                    if (!isNaN(item.expand.max)) {
                        var iMax = parseInt(item.expand.max, 10);
                        max = max > iMax ? iMax : max;
                    }
                }

                //创建其余项,绑定默认选中值
                $(item.data).each(function (i, detail) {
                    //判断当前项是否为默认选中项
                    if (isHasExpandCallBack && (1 + i) > max) {
                        return;
                    }
                    strHTML += '<span title="{0}" data-id="{1}" data-value="{3}" {2}>{0}</span>'.format(detail[item.textField], i, $.inArray(detail[item.valueField], item.defaults) >= 0 ? "class='selected'" : "", detail[item.valueField]);
                });

                return strHTML;
            }

            //创建自定义查询条件选项
            function _createCustomFilter(i, item) {
                if (item.custom) {
                    var inputwidth = "70px";
                    if (item.type == "date") {
                        inputwidth = "100px";
                        //为年月日类型
                    } else if (item.type == "datetime") {
                        //为年月日时分秒类型
                        inputwidth = "160px";
                    }
                    var strHTML = '<div class="filter_custom" style="width:{0}px;"><span>自定义</span>'.format(_getCustomDivWidth(item));
                    strHTML += '<span><input type="text" id="{0}_c_custom_start" style="width:{1};"></span>'.format(item.id, inputwidth);
                    if (item.custom.isRange) {
                        //范围
                        strHTML += '<span>—</span>' +
                            '<span><input type="text" id="{0}_c_custom_end" {1}></span>'.format(item.id, inputwidth ? "style='width:{0}'".format(inputwidth) : "");
                    }
                    strHTML += '<span class="btn_filter_sure" data-id="{0}">确定</span></div>'.format(i);
                    return strHTML;
                } else {
                    return "";
                }
            }

            //获取当前查询框ID
            function _getItemIndex(objthis) {
                return $(objthis).closest('.searchbox-item').attr("data-id");
            }

            /*
            * 功能：    获取当前查询框绑定项
            * 参数：    当前项元素
            * 返回值：  当前项数据
            * 创建人：  杜冬军
            * 创建时间：2015-12-21
            */
            function _getItem(objthis) {
                var index = _getItemIndex(objthis);
                return settings.searchBoxs[index];
            }

            /*
            * 功能：    获取当前查询框最终搜索条件
            * 参数：    无
            * 返回值：  搜索条件
            * 创建人：  杜冬军
            * 创建时间：2015-12-24
            */
            function _getParamList() {
                var paramList = [];
                var value = null;
                $(settings.searchBoxs).each(function (i, item) {
                    value = {};
                    if (item.customSelectd.length > 0) {
                        //自定义
                        value[settings.paramCustomkey] = item.customSelectd;

                    } else {
                        value[settings.paramkey] = item.selected;
                    }
                    value["isMultiple"] = item.isMultiple;
                    value["id"] = item.srcID;
                    paramList.push(value);
                });

                return paramList;
            }

            /*
            * 功能：    获取当前查询框是否含有展开收缩回调事件
            * 参数：    当前项元素
            * 返回值：  bool
            * 创建人：  杜冬军
            * 创建时间：2015-12-21
            */
            function _isHasExpandEvent(item) {
                return item.expand && (typeof (item.expand.event) == "function");
            }

            /*
            * 功能：    每一个查询条件后面展开收缩监听事件处理
            * 参数：    event：event
            *           data：展开还是收缩
            *           that: 当前展开收缩元素
            * 返回值：  bool
            * 创建人：  杜冬军
            * 创建时间：2015-12-21
            */
            function _itemExpand(event, that, data) {
                event.cancelBubble = true;

                var state = _getExpandState(that);
                if (data && data == state) {
                    return;
                }

                var objFont = $(that).find(".fa");
                var objcenter = $(that).siblings(".c");

                if (state == "expand") {
                    $(that).find(".text").text("收缩");
                    objFont.removeClass("fa-angle-double-down").addClass("fa-angle-double-up");
                    $(that).siblings(".c").css({ "height": "auto" });
                } else {
                    $(that).find(".text").text("展开");
                    objFont.removeClass("fa-angle-double-up").addClass("fa-angle-double-down");
                    objcenter.css({ height: 30 });
                }
                //修复如果有多个展开条件时，搜索框高度自适应问题
                if (filterBtn.find('span').text() == "展开条件") {
                    var expandItemNum = 0;
                    var expandheight = 0;
                    $(".searchbox-item").each(function (i, item) {
                        if ($(item).find(".text").text() == "收缩") {
                            expandItemNum++;
                            expandheight += $(item).find(".c").height();
                        }
                    });
                    var height = (settings.expandRow - expandItemNum) * 40 + 9 * expandItemNum + expandheight;
                    if (expandItemNum == 0) {
                        height = _expandHeight;
                    }
                    searchCtl.css({ 'height': height });
                }

                var item = _getItem(that);

                //回调事件
                if (_isHasExpandEvent(item)) {
                    item.expand.event(item.data, that, state);
                }


                /*
                * 功能：    选项选中取消监听事件处理
                * 参数：    that:
                *           dataid：选项data-id序号
                *           state：true 选中 false 取消
                * 返回值：  无
                * 创建人：  杜冬军
                * 创建时间：2015-12-25
                */
                function _changeState(that, dataid, state) {

                }

                /*
                * 功能：    获取当前展开收缩按钮状态
                * 参数：    当前项元素
                * 返回值：  bool
                * 创建人：  杜冬军
                * 创建时间：2015-12-21
                */
                function _getExpandState(obj) {
                    var objFont = $(obj).find(".fa");
                    if (objFont.hasClass("fa-angle-double-down")) {
                        return "expand";
                    } else {
                        return "collaspe";
                    }
                }
            }

            /*
           * 功能：   清空自定义查询框的值
           * 参数：   item  当前项元素
           * 返回值：  无
           * 创建人：  杜冬军
           * 创建时间：2015-12-25
           */
            function _clearCustomValue(item) {
                if (item.custom && item.customSelectd.length > 0) {
                    item.customSelectd = [];
                    //清除输入框的值
                    $("#{0}_c_custom_start".format(item.id)).val('');

                    if (item.custom.isRange) {
                        $("#{0}_c_custom_end".format(item.id)).val('');
                    }
                }
            }

            /*
            * 功能：   设置自定义查询框值
            * 参数：   item  当前项元素
            * 返回值：  无
            * 创建人：  杜冬军
            * 创建时间：2015-12-25
            */
            function _setCustomValue(item) {
                if (item.custom && item.customSelectd.length > 0) {
                    //清除输入框的值
                    $("#{0}_c_custom_start".format(item.id)).val(item.customSelectd[0]);
                    if (item.custom.isRange) {
                        $("#{0}_c_custom_end".format(item.id)).val(item.customSelectd[1]);
                    }
                }
            }


            /*
            * 功能：   重新给searchBox赋值
            * 参数：   arrOptionValue  每个过滤项值
            * 返回值：  无
            * 创建人：  杜冬军
            * 创建时间：2016-09-08
            */
            function _setSearchValue(arrOptionValue) {
                if ($.isArray(arrOptionValue)) {
                    var jsonMapper = {}, itemSet = null;
                    for (var i = 0, length = arrOptionValue.length; i < length; i++) {
                        itemSet = arrOptionValue[i];
                        jsonMapper[itemSet.id] = itemSet;
                    }
                    var itemSpans;

                    $(settings.searchBoxs).each(function (i, item) {
                        itemSet = jsonMapper[item.srcID];
                        //所有选项
                        itemSpans = $("#" + item.id).find(".filter_option span");
                        //清除当前选中
                        itemSpans.removeClass("selected");
                        //清除自定义选中值
                        _clearCustomValue(item);
                        //清除选中值
                        item.selected = [];

                        if (!itemSet) {
                            restoreToDefault(itemSpans, item);
                        } else {
                            var valueList = itemSet[settings.paramkey];
                            var customValueList = itemSet[settings.paramCustomkey];
                            if (valueList && valueList.length > 0) {
                                //选项赋值
                                for (var i = 0; i < valueList.length; i++) {
                                    itemSpans.filter("[data-value='{0}']".format(valueList[i])).addClass("selected");
                                    item.selected.push(valueList[i]);
                                }
                            } else if (customValueList && customValueList.length > 0) {
                                //自定义选中赋值
                                for (var i = 0; i < customValueList.length; i++) {
                                    item.customSelectd.push(customValueList[i]);
                                }
                                _setCustomValue(item);
                            } else {
                                restoreToDefault(itemSpans, item);
                            }
                        }
                    });
                }

                //还原单项到默认状态
                function restoreToDefault(itemSpans, item) {
                    //该选项还原默认值
                    if (item.defaults.length == 0) {
                        //选中全部
                        itemSpans.filter(".option_all").addClass("selected");
                    } else {
                        for (var i = 0; i < item.defaults.length; i++) {
                            itemSpans.filter("[data-value='{0}']".format(item.defaults[i])).addClass("selected");
                            item.selected.push(item.defaults[i]);
                        }
                    }
                }
            }
            ////////////////////////////////////////私有方法////////////////////////////////////////

            return searchCtl.each(function () {
                this.getParamList = _getParamList;
                this.setValue = _setSearchValue;
                this.isSeachBox = true;
            });
        },
        /*
        * 功能：    获取搜索条件参数
        * 参数：    无
        * 返回值：  搜索条件参数
        * 创建人：  杜冬军
        * 创建时间：2015-12-24
        */
        getParamList: function () {
            var that = this[0];
            if (that.isSeachBox) {
                return that.getParamList();
            }
        },
        /*
         * 功能：    searchBox对外提供的调用函数
         * 参数：    options {"setValue":[]}  key为要调用的函数名称 value:为函数调用参数
         * 返回值：  函数返回值
         * 创建人：  杜冬军
         * 创建时间：2016-09-08
        */
        searchFunctionCall: function (options) {
            if ($.isPlainObject(options)) {
                var that = this[0];
                if (that.isSeachBox) {
                    for (var key in options) {
                        if ($.isFunction(that[key])) {
                            return that[key](options[key]);
                        } else {
                            console.error("查询插件searchBox不支持“{0}”方法".format(key));
                            return null;
                        }
                    }
                }
            }
        }
    });
})(jQuery);


jQuery.extend({
    download: function (url, data, method) {
        if (url && data) {
            method = method || 'post';
            data = typeof (data) == "string" ? data : decodeURIComponent($.myParam(data));
            var inputs = '';
            $.each(data.split('&'), function () {
                var pair = this.split('=');
                inputs += '<input type="hidden"   name="{0}" value="{1}"/>'.format(pair[0], pair[1]);
            });
            var objForm = $("#fileForm");
            if (objForm.length == 0) {
                objForm = $('<form id="fileForm" method="{0}" target="fileIFrame" action="{1}">{2}</form><iframe id="fileIFrame" name="fileIFrame" style="display:none;"></iframe>'.format(method, url, inputs)).appendTo('body');
            } else {
                objForm.attr("method", method).attr("action", url).html(inputs);
            }
            objForm.submit();
        }
    },
    isJson: function (obj) {
        var isjson = typeof (obj) == "object" && Object.prototype.toString.call(obj).toLowerCase() == "[object object]" && !obj.length;
        return isjson;
    },
    myParam: function (a, traditional) {
        var prefix, s = [], rbracket = /\[\]$/,
            add = function (key, value) {
                value = jQuery.isFunction(value) ? value() : (value == null ? "" : value);
                s[s.length] = encodeURIComponent(key) + "=" + encodeURIComponent(value);
            },
        buildParams = function (prefix, obj, traditional, add) {
            var name;
            if (jQuery.isArray(obj)) {
                jQuery.each(obj, function (i, v) {
                    if (traditional || rbracket.test(prefix)) {
                        add(prefix, v);
                    } else {
                        // Item is non-scalar (array or object), encode its numeric index.
                        buildParams(prefix + "[" + (typeof v === "object" ? i : "") + "]", v, traditional, add);
                    }
                });

            } else if (!traditional && jQuery.type(obj) === "object") {
                // Serialize object item.
                for (name in obj) {
                    buildParams(prefix + "[" + name + "]", obj[name], traditional, add);
                }

            } else {
                // Serialize scalar item.
                add(prefix, obj);
            }
        };

        if (traditional === undefined) {
            traditional = jQuery.ajaxSettings && jQuery.ajaxSettings.traditional;
        }

        if (jQuery.isArray(a) || (a.jquery && !jQuery.isPlainObject(a))) {
            // Serialize the form elements
            jQuery.each(a, function () {
                add(this.name, this.value);
            });

        } else {
            for (prefix in a) {
                buildParams(prefix, a[prefix], traditional, add);
            }
        }

        // Return the resulting serialization
        return s.join("&");
    },

    /*
    * 功能：    加载js,css文件
    * 参数：    name：文件名称
    * 返回值：  bool
    * 创建人：  杜冬军
    * 创建时间：2016-01-25
    */
    loadFile: function (name) {
        var js = /js$/i.test(name);
        var bInclude = false;
        var tag = js ? 'script' : 'link';
        var attr = js ? 'src' : 'href';
        var es = document.getElementsByTagName(tag);
        for (var i = 0; i < es.length; i++) {
            if (es[i][attr].indexOf(name) != -1) {
                bInclude = true;
                break;
            }
        }
        if (!bInclude) {
            $(js ? 'body' : 'head').append('<{0} {3} {1}="{2}"></{0}>'.format(tag, attr, name, js ? 'type="text/javascript"' : 'rel="stylesheet"'));
        }
    },

    /*
    * 功能：    生成guid
    * 参数：    无
    * 返回值：  guid
    * 创建人：  杜冬军
    * 创建时间：2016-01-25
    */
    UUID: function () {
        var s = [], itoh = '0123456789ABCDEF'.split('');
        for (var i = 0; i < 36; i++) s[i] = Math.floor(Math.random() * 0x10);
        s[14] = 4;
        s[19] = (s[19] & 0x3) | 0x8;
        for (var i = 0; i < 36; i++) s[i] = itoh[s[i]];
        s[8] = s[13] = s[18] = s[23] = '-';
        return s.join('');
    },

    /*
    * 功能：    获取url参数值
    * 参数：    url 地址
    * 返回值：  参数键值对
    * 创建人：  杜冬军
    * 创建时间：2016-01-26
    */
    getUrlParms: function (url) {
        var paramsArray = {};
        var fisrtIndex = url.indexOf("?");
        if (fisrtIndex + 1 < url.length) {
            var tmparray = url.substring(fisrtIndex + 1).split("&");
            var reg = /[=|^==]/;
            if (tmparray != null) {
                for (var i = 0; i < tmparray.length; i++) {
                    var set1 = tmparray[i].replace(reg, '&');
                    var tmpStr2 = set1.split('&');
                    paramsArray[tmpStr2[0]] = unescape(tmpStr2[1]);
                }
            }
        }
        // 将参数数组进行返回
        return paramsArray;
    },

    /*
    * 功能：    导出数据到EXCEL下载
    * 参数：    options：参数  格式如下
                {
                   columnInfoList:[{
                            "Align": "left", /取值 left right center
                            "Header": "标题头",
                            "Field": "绑定域"  //绑定字段名称
                        }]
                   "api":取数接口地址,  如果该参数有值则会进行后台取数  
                   condition":后台取数查询参数
                   "data": 前台传给后台的数据，
                   "type":get 还是post 默认post
                   fileName:导出文件名
                }
    * 返回值：  无
    * 创建人：  杜冬军
    * 创建时间：2016-02-22
    */
    ExportXLS: function (options) {
        if (options.columnInfos && options.columnInfos.length > 0) {
            if ($("#excelForm").length == 0) {
                $('<form id="excelForm"  method="post" target="excelIFrame"><input type="hidden" name="excelParam" id="excelData" /></form><iframe id="excelIFrame" name="excelIFrame" style="display:none;"></iframe>').appendTo("body");
            }

            var param = { "FileName": options.fileName, "ColumnInfoList": options.columnInfos, "Type": options.type };
            //默认不分页导出数据
            if (!$.isArray(options.data)) {
                //查询条件
                var queryParam = Ywdsoft.clone(options.condition);
                //最多导出50000条数据
                queryParam.pagesize = 20000;
                queryParam.page = 1;

                param.Condition = queryParam;
                param.RootPath = document.location.origin;
                param.Api = document.location.origin + options.api;
                param.IsExportSelectData = false;
            } else {
                param.Data = options.data;
                param.IsExportSelectData = true;
            }

            $("#excelData").val(JSON.stringify(param));
            var excelForm = document.getElementById("excelForm");
            excelForm.action = "/Excel/GridExport";
            excelForm.submit();
        }
    },
    /*
    * 功能：    根据业务类型下载导入数据得模版文件
    * 参数：    type：业务类型 取值参照 Ywdsoft.Utility.Excel.ExcelImportType 枚举
    * 返回值：  无
    * 创建人：  焰尾迭
    * 创建时间：2016-08-19
    */
    DownloadExcelTemplate: function (type) {
        if (type == "undefined") {
            return;
        }
        var param = { type: type };
        $.download("/Excel/DownLoadTemplate", param, "get");
    },
    /*
    * 功能：    根据业务类型下载导入数据得模版文件
    * 参数：    options：
                {
                    type:业务类型, 取值参照 Ywdsoft.Utility.Excel.ExcelImportType 枚举
                    FunctionCode:业务模块Code,
                    Ext:可导入文件类型,
                    ReturnDetailData:是否返回详细数据
                    after:function(){}//回调函数
                }
    * 返回值：  无
    * 创建人：  焰尾迭
    * 创建时间：2016-08-22
    */
    ImportExcelTemplate: function (options) {
        if ($.isPlainObject(options)) {
            var defaults = {
                ReturnDetailData: 0
            };

            var param = $.extend({}, defaults, options);

            if (param.type != "undefined") {
                //加载样式和js文件
                $.loadFile("/Content/Css/plugins/webuploader/webuploader.css");
                $.loadFile("/Content/Scripts/plugins/webuploader/webuploader.min.js");
                if (!WebUploader.Uploader.support()) {
                    var error = "上传控件不支持您的浏览器！请尝试升级flash版本或者使用Chrome引擎的浏览器。<a target='_blank' href='http://www.chromeliulanqi.com'>下载页面</a>";
                    if (window.console) {
                        window.console.log(error);
                    }
                    return;
                }

                var id = "ImportExcelTemplate{0}".format(param.type);
                var modal = $("#" + id);
                $(modal).remove();
                var html =
                    '<div class="modal" id="{0}">'.format(id) +
                        '<div class="modal-dialog">' +
                            '<div class="modal-content">' +
                                '<div class="modal-header">' +
                                    '<button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>' +
                                    '<h4 class="modal-title">Excel导入</h4>' +
                                '</div>' +
                                '<div class="modal-body">' +
                                    '<div id="uploader" class="wu-example">' +
                                        '<p style="font-weight:bold;">导入说明:</p><p class="pt5">导入文件为EXCEL格式，请先下载模板进行必要信息填写，模板下载<a href="javascript:;" onclick="$.DownloadExcelTemplate(\'{0}\')">请点击这里</a>！</p>'.format(param.type) +
                                        '<div id="thelist" class="uploader-list"></div>' +
                                        '<div class="uploader-wrap clearfix pb20">' +
                                        '<input type="text" readonly class="form-control input-sm mr5 upload-file-name" style="width:300px;" />' +
                                        '<div id="picker">选择文件</div>' +
                                        '<button id="ctlBtn" class="btn btn-white btn-sm btn-start-uploader ml5" style="display:none;">开始上传</button>' +
                                        '</div>'
                '</div>' +
            '</div></div></div></div>';
                $(html).appendTo("body");
                modal = $("#" + id);
                var postData = { type: param.type, FunctionCode: param.FunctionCode, ReturnDetailData: param.ReturnDetailData };
                var uploader = WebUploader.create({
                    swf: '/Content/Scripts/plugins/webuploader/Uploader.swf',
                    server: '/Excel/ImportTemplate?' + $.param(postData),
                    pick: '#picker',
                    accept: {
                        title: 'excel',
                        extensions: 'xls',
                        mimeTypes: 'application/msexcel'
                    },
                    resize: false,
                    fileSingleSizeLimit: 10 * 1024 * 1024,//10M
                    duplicate: true
                });

                $("#ctlBtn").on('click', function () {
                    uploader.upload();
                });

                // 当有文件被添加进队列的时候
                uploader.on('fileQueued', function (file) {
                    $("#thelist").html('<div id="' + file.id + '" class="item">' +
                        '<div class="state"></div>' +
                    '</div>');
                    $(".upload-file-name").val(file.name);
                    $(".btn-start-uploader").show();
                });

                // 文件上传过程中创建进度条实时显示。
                uploader.on('uploadProgress', function (file, percentage) {
                    var $li = $('#' + file.id),
                        $percent = $li.find('.progress .progress-bar');

                    // 避免重复创建
                    if (!$percent.length) {
                        $percent = $('<div class="progress progress-striped active">' +
                          '<div class="progress-bar" role="progressbar" style="width: 0%">' +
                          '</div>' +
                        '</div>').appendTo($li).find('.progress-bar');
                    }

                    $li.find('.state').text('上传中');

                    $percent.css('width', percentage * 100 + '%');
                    $(".upload-file-name").val("");
                    $(".btn-start-uploader").hide();
                });

                uploader.on('uploadSuccess', function (file, response) {
                    if (response.IsSuccess) {
                        $('#' + file.id).find('.state').html('<span class="label label-success">' + response.Message + '</span>');
                        if ($.isFunction(param.after)) {
                            param.after(response, modal);
                        }
                    } else {
                        if (response.Message.indexOf("http://") >= 0) {
                            $('#' + file.id).find('.state').html("上传的数据中存在错误数据，请点击<a class='red' href='{0}' target='_blank'>下载错误数据</a>！".format(response.Message));
                        } else {
                            $('#' + file.id).find('.state').html('<span class="label label-danger" title="' + response.Message + '">' + response.Message + '</span>');
                        }
                    }


                });

                uploader.on('uploadError', function (file, response) {
                    console.log(response);
                    $('#' + file.id).find('.state').text('上传出错');
                });

                uploader.on('uploadComplete', function (file) {
                    $('#' + file.id).find('.progress').fadeOut(200);
                });

                modal.modal('show');
            }
        }
    }
});


/***************************************************** 控件拓展   ********************************************************************************/

/***************************************************** JS数据类型方法拓展   ********************************************************************************/
/*
* 功能：    拓展String类型方法，添加常用功能
* 创建人：  杜冬军
* 创建时间：2015-11-18
*/
$.extend(String.prototype, {
    /*
    * 功能：    类似C# String.Format()格式化功能
    * 参数：    args：参数
    * 返回值：  无
    */
    format: function (args) {
        var result = this;
        if (arguments.length > 0) {
            if (arguments.length == 1 && typeof (args) == "object") {
                for (var key in args) {
                    if (args[key] != undefined) {
                        var reg = new RegExp("({" + key + "})", "g");
                        result = result.replace(reg, args[key]);
                    }
                }
            }
            else {
                for (var i = 0; i < arguments.length; i++) {
                    if (arguments[i] != undefined) {
                        //var reg = new RegExp("({[" + i + "]})", "g");//这个在索引大于9时会有问题，谢谢何以笙箫的指出

                        var reg = new RegExp("({)" + i + "(})", "g");
                        result = result.replace(reg, arguments[i]);
                    }
                }
            }
        }
        return result;
    },
    /*
    * 功能：    类似C# String.PadLeft()左对齐自动补全功能
    * 参数：    args：参数  
                totalWidth 总宽度
                paddingChar 替换字符
    * 返回值：  无
    */
    padLeft: function () {
        var result = this;
        if (arguments.length > 0) {
            if (arguments.length >= 2) {
                var totalWidth = arguments[0];
                var paddingChar = arguments[1];
                var len = result.toString().length;
                while (len < totalWidth) {
                    result = paddingChar + result;
                    len++;
                }
            }
        }
        return result;
    },
    /*
    * 功能：    类似C# String.PadLeft()右对齐自动补全功能
    * 参数：    args：参数  
                totalWidth 总宽度
                paddingChar 替换字符
    */
    padRight: function () {
        var result = this;
        if (arguments.length > 0) {
            if (arguments.length >= 2) {
                var totalWidth = arguments[0];
                var paddingChar = arguments[1];
                var len = result.toString().length;
                while (len < totalWidth) {
                    result = paddingChar + result;
                    len++;
                }
            }
        }
        return result;
    },
    /*
    * 功能：    获取字符串的字节数
    * 返回值：  字节长度
    */
    getByteLength: function () {
        return this.replace(/[^\x00-\xff]/g, "ci").length;
    },
    /*
    * 功能：    去掉字符串两边的空格
    * 返回值：  去掉字符串两边的空格
    */
    trim: function () {
        return this.replace(/(^\s*)|(\s*$)/g, '');
    },
    /*
    * 功能： 将字符串转换成bool类型  1 和true认为是true
    */
    toBool: function () {
        var str = this.toString().toLocaleLowerCase();
        if (str === "1" || str === "true") {
            return true;
        } else if (str === "0" || str === "false") {
            return false;
        }
    },
    /*
    * 功能： 将字符串转化为整数
    */
    toInt: function () {
        return parseInt(this, 10);
    },
    /*
    * 功能： 将字符串转化为浮点数
    * 参数： round：小数保留位
    */
    toFloat: function (round) {
        var num = this.replace(/,/g, "");
        var res = (isNaN(num) || num == "") ? 0 : parseFloat(num);
        return round ? res.round(round) : res;
    },
    toDate: function () {
        if (this) {
            return Ywdsoft.parseDate(this.toString());
        }
        return null;
    },
    formatDateString: function (format) {
        format = format || "yyyy-MM-dd";
        if (this) {
            return Ywdsoft.formatDate(Ywdsoft.parseDate(this.toString()), format);
        }
        return null;
    },
    escapeDateTimeTokens: function () {
        return this.replace(/([dMyHmsft])/g, "\\$1");
    },
    /*
   * 功能：    日期加分钟
   * 创建人：  杜冬军
   * 创建时间：2016-01-15
   */
    addMinutes: function (minutes, format) {
        format = format || "yyyy-MM-dd HH:mm:ss";
        var Digital = this.toDate();
        return Digital.addMinutes(minutes).toString().formatDateString(format);
    },
    /*
    * 功能：    日期加天
    * 创建人：  杜冬军
    * 创建时间：2016-01-15
    */
    addDays: function (days, format) {
        return this.addMinutes(days * 24 * 60, format);
    }
});

/*
* 功能：    拓展Number类型方法，添加常用功能
* 创建人：  杜冬军
* 创建时间：2015-11-18
*/
$.extend(Number.prototype, {
    /*
    * 功能：数字进行特定保留位处理
    * 参数：n:小数位数
    */
    round: function (n) {
        n = Math.pow(10, n || 0).toFixed(n < 0 ? -n : 0);
        return Math.round(this * n) / n;
    },
    /// <summary>加法运算</summary>
    /// <param name="num" type="float">被加数</param>
    /// <param name="round" type="int">保留几位小数</param>
    add: function (num, round) {
        num = num.toString().replace(/,/g, "");
        if (isNaN(num)) { throw "被加数不能转化成数字" }
        round = round == undefined ? 2 : round.toString().toInt();
        num = num.toFloat();
        var r1 = 0, r2 = 0, arr1 = this.toString().split("."), arr2 = num.toString().split(".");
        if (arr1.length > 1) { r1 = arr1[1].length };
        if (arr2.length > 1) { r2 = arr2[1].length };
        var m = Math.pow(10, Math.max(r1, r2));
        return ((this * m + num * m) / m).round(round);
    },
    /// <summary>减法运算</summary>
    /// <param name="num" type="float">被减数</param>
    /// <param name="round" type="int">保留几位小数</param>
    sub: function (num, round) {
        num = num.toString().replace(/,/g, "");
        if (isNaN(num)) { throw "被加数不能转化成数字" }
        round = round == undefined ? 2 : round.toString().toInt();
        num = num.toFloat();
        var r1 = 0, r2 = 0, arg1 = this.toString().split("."), arg2 = num.toString().split(".");
        if (arg1.length > 1) { r1 = arg1[1].length };
        if (arg2.length > 1) { r2 = arg2[1].length };
        var m = Math.pow(10, Math.max(r1, r2));
        return ((this * m - num * m) / m).round(round);
    },
    /// <summary>乘法运算</summary>
    /// <param name="num" type="float">被乘数</param>
    /// <param name="round" type="int">保留几位小数</param>
    mul: function (num, round) {
        num = num.toString().replace(/,/g, "");
        if (isNaN(num)) { throw "被加数不能转化成数字" }
        round = round == undefined ? 2 : round.toString().toInt();
        num = num.toFloat();
        var r1 = 0, r2 = 0, arg1 = this.toString().split("."), arg2 = num.toString().split(".");
        if (arg1.length > 1) { r1 = arg1[1].length };
        if (arg2.length > 1) { r2 = arg2[1].length };
        return ((this * Math.pow(10, r1) * num * Math.pow(10, r2)) / Math.pow(10, r1.add(r2))).round(round);
    },
    /// <summary>除法运算</summary>
    /// <param name="num" type="float">被除数</param>
    /// <param name="round" type="int">保留几位小数</param>
    div: function (num, round) {
        num = num.toString().replace(/,/g, "");
        if (isNaN(num)) { throw "被加数不能转化成数字" }
        else if (new Number(num) == 0) {
            return "";
        }
        round = round == undefined ? 2 : round.toString().toInt();
        var r1 = 0, r2 = 0, arr1 = this.toString().split("."), arr2 = num.toString().split(".");
        if (arr1.length > 1) { r1 = arr1[1].length };
        if (arr2.length > 1) { r2 = arr2[1].length };
        var m = Math.pow(10, Math.max(r1, r2));
        return ((this * m) / (num * m)).round(round);
    },
    formatNum: function (n) {
        var s = this;
        if (n == 0) {
            return parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n);
        }
        else {
            n = n > 0 && n <= 20 ? n : 2;
            s = parseFloat((s + "").replace(/[^\d\.-]/g, "")).toFixed(n) + "";
            var l = s.split(".")[0].split("").reverse(),
        r = s.split(".")[1];
            t = "";
            for (i = 0; i < l.length; i++) {
                t += l[i] + ((i + 1) % 3 == 0 && (i + 1) != l.length ? "," : "");
            }
            return t.split("").reverse().join("") + "." + r;
        }
    }
});

$.extend(Date.prototype, {
    getHalfYear: function () {
        if (!this.getMonth) return null;
        var m = this.getMonth();
        if (m < 6) return 0;
        return 1;
    },
    getQuarter: function () {
        if (!this.getMonth) return null;
        var m = this.getMonth();
        if (m < 3) return 0;
        if (m < 6) return 1;
        if (m < 9) return 2;
        return 3;
    },
    /*
    * 功能：    日期加分钟
    * 创建人：  杜冬军
    * 创建时间：2016-01-15
    */
    addMinutes: function (minutes) {
        var t = this.getTime();
        t += minutes * 60 * 1000;
        this.setTime(t);
        return this;
    },
    /*
    * 功能：    日期加天
    * 创建人：  杜冬军
    * 创建时间：2016-01-15
    */
    addDays: function (days) {
        return this.addMinutes(days * 24 * 60);
    }
});
/***************************************************** JS数据类型方法拓展   ********************************************************************************/


