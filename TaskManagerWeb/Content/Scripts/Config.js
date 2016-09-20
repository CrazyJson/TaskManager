Namespace.register("Ywdsoft.Config");
Ywdsoft.Config = (function () {
    var DicCName = null, BaseData = null, form = null, objForm = null;

    //初始化数据
    initData = function () {
        $.ajax({
            type: "get",
            url: "/Config/GetAllOption",
            dataType: "json",
            beforeSend: function () {
                //加载等待层
                index = layer.load(0);
            },
            complete: function () {
                layer.close(index);
            },
            success: function (data) {
                BaseData = data;
                drawConfig(BaseData);
            }
        });
    },

    //根据数据绘制配置主页面
    drawConfig = function (data) {
        var html = template('Config_Index', { "Result": data });
        $("#main-table").html(html);
    },

    //事件绑定
    bindEvents = function () {
        //单向点击 打开编辑界面事件
        $("#main-table").on("click", "a.Dnamelink", function (e) {
            var id = $(this).attr("id");
            var configName = $(this).text();
            $("#form_GroupType").val(id);
            var data = null;
            for (var i = 0, length = BaseData.length; i < length; i++) {
                data = BaseData[i];
                if (data.Group.GroupType == id) {
                    break;
                }
            }
            //标题
            $(".userTitles").text("修改{0}".format(configName));
            //内容
            var html = template('Config_Detail', data);
            objForm.html(html);

            Ywdsoft.parse();
            form = new Ywdsoft.Form("CinfigForm")
            //显示
            $("#addConfig").modal('show');
        });

        //保存按钮事件
        $("#btn_save").on("click", function () {
            saveForm();
        });

        //保存按钮事件
        $("#btn_saveAndClose").on("click", function () {
            saveForm(true);

        });

        //查询事件
        //全局变量用于标识是否延时执行keyup事件
        var flag;
        $("#GroupName_Search").on("keyup", function () {
            clearTimeout(flag);
            //延时400ms执行请求事件，如果感觉时间长了，就用合适的时间
            //只要有输入则不执行keyup事件
            flag = setTimeout(function () {
                searchGroup();
            }, 400);
        });

        function searchGroup() {
            var item = null, key = $("#GroupName_Search").val().trim();
            var _data = [];
            for (var i = 0, length = BaseData.length; i < length; i++) {
                item = BaseData[i];
                if (item.Group.GroupName.indexOf(key) >= 0) {
                    _data.push(item);
                }
            }
            drawConfig(_data);
        }

        //保存数据
        //参数 isCloseWin:是否关闭当前窗口
        function saveForm(isCloseWin) {
            isCloseWin = isCloseWin || false;
            var data = form.getData();
            if (!form.validate()) {
                return;
            }
            //转换数据
            var strGroupType = $("#form_GroupType").val();
            var ajaxData = { Group: { GroupType: strGroupType }, ListOptions: [] };
            var objInput = null;
            for (var key in data) {
                objInput = objForm.find('[name="{0}"]'.format(key));
                ajaxData.ListOptions.push({
                    OptionType: strGroupType,
                    OptionName: objInput.attr("OptionName"),
                    Key: key,
                    Value: data[key].trim(),
                    ValueType: objInput.attr("ValueType")

                });
            }
            var index;
            $.ajax({
                type: "post",
                url: "/Config",
                data: JSON.stringify(ajaxData),
                contentType: "application/json",
                dataType: "json",
                beforeSend: function () {
                    index = layer.load(0);
                },
                complete: function () {
                    layer.close(index);
                },
                success: function (data) {
                    if (data.HasError) {
                        layer.alert(data.Message);
                        return;
                    }
                    refreshData(strGroupType);
                    if (isCloseWin) {
                        $('#addConfig').modal('hide');
                    } else {
                        layer.msg("配置保存成功");
                    }
                }
            });
        }

        //保存成功后刷新前端数据
        function refreshData(groupType) {
            $.ajax({
                type: "get",
                url: "/Config/GetOptionByGroup",
                data: { GroupType: groupType },
                dataType: "json",
                beforeSend: function () {
                    index = layer.load(0);
                },
                complete: function () {
                    layer.close(index);
                },
                success: function (data) {
                    var item = null;
                    for (var i = 0, length = BaseData.length; i < length; i++) {
                        item = BaseData[i];
                        if (item.Group.GroupType == groupType) {
                            BaseData[i] = data;
                            break;
                        }
                    }
                }
            });
        }
    };

    return {
        /*界面初始化*/
        init: function () {
            //初始化数据
            initData();
            objForm = $("#CinfigForm");
            bindEvents();
        }
    }
})();
