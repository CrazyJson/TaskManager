Namespace.register("Ywdsoft.Config");
Ywdsoft.Config = (function () {
    var DicCName = null, BaseData = null, form = null, objForm = null,

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

    //获取热门标签
    drawHeatTag = function (tagName) {
        var item = null, itemTag = null, tagList = [], jsonKey = {};
        for (var i = 0, length = BaseData.length; i < length; i++) {
            item = BaseData[i].TagList;
            if (item != null && item.length > 0) {
                for (var j = 0, l = item.length; j < l; j++) {
                    itemTag = item[j];
                    if (!jsonKey.hasOwnProperty(itemTag.TagName)) {
                        tagList.push(itemTag);
                        jsonKey[itemTag.TagName] = 1;
                    } else {
                        jsonKey[itemTag.TagName] = ++jsonKey[itemTag.TagName];
                    }
                }
            }
        }

        var bExists = false;
        for (var i = 0, l = tagList.length; i < l; i++) {
            item = tagList[i];
            item.ConfigCount = jsonKey[item.TagName];
            if (item.TagName == tagName) {
                bExists = true;
            }
        }

        tagList.sort(function (a, b) {
            return (a.TagName + '').localeCompare(b.TagName + '');

        });
        if (tagList.length == 0) {
            $("#tagContainer").addClass("undis");
        } else {
            $("#tagContainer").removeClass("undis");
        }
        $("#TagsCloud").html(template('Config_Tag', { "TagList": tagList, "TagName": tagName }));
        if (!bExists) {
            var html = template('Config_Index', { "Result": BaseData });
            $("#main-table").html(html);
        }
    },

    //根据数据绘制配置主页面
    drawConfig = function (data) {
        drawHeatTag($("#TagsCloud").find("span.selected").attr("data-text"));
        var html = template('Config_Index', { "Result": data });
        $("#main-table").html(html);
    },

    //新增自定义标签
    addLabel = function (tag) {
        //删除所有空格
        tag = tag.replace(/\s/g, "");
        if (!tag) {
            return false;
        }
        var $label = $("#ListLabelCloud .list-label");
        //判断标签是否已经存在
        for (var i = 0, length = $label.length; i < length; i++) {
            if ($label.eq(i).text() == tag) {
                return false;
            }
        }
        if ($label.length >= 10) {
            layer.msg("最多添加10个标签！", { icon: 5, time: 4000 });
            return false
        }
        $("#ListLabelCloud").append('<span class="list-label">{0}<a href="javascript:;" class="action-delete fa fa-times"></a></span>'.format(tag)).removeClass("undis");
        $("#labelText").val('');
        return true
    },
    //更新标签热度
    updateHeat = function (TagGUID) {
        $.ajax({
            type: "get",
            url: "/Tags/UpdateTagHeat",
            data: { TagGUID: TagGUID }
        });
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

            if (data.Group.CustomPage) {
                //自定义配置界面跳转
                var area = ['100%', '100%'];
                layer.open({
                    move: false,
                    anim: 1,
                    type: 2,
                    title: '修改' + data.Group.GroupName,
                    shade: 0.8,
                    area: area,
                    content: data.Group.CustomPage
                });
            } else {
                //标题
                $(".userTitles").text("修改{0}".format(configName));
                //内容
                var html = template('Config_Detail', data);
                objForm.html(html);

                Ywdsoft.parse();
                form = new Ywdsoft.Form("CinfigForm")
                //显示
                $("#addConfig").modal('show');
            }
        });

        //保存按钮事件
        $("#btn_save").on("click", function () {
            saveForm();
        });

        //保存按钮事件
        $("#btn_saveAndClose").on("click", function () {
            saveForm(true);

        });

        //标签数据框输入完成回车事件
        $("#CinfigForm").on("keyup", '#labelText', function (e) {
            if (e.keyCode == 13) {
                addLabel($(this).val());
            }
        });

        //添加自定义标签
        $("#CinfigForm").on("click", '#btn-addLabel', function () {
            addLabel($("#labelText").val());
        });

        //删除自定义标签
        $("#CinfigForm").on("click", '#ListLabelCloud .action-delete', function () {
            $(this).parent('.list-label').remove();
            if ($("#ListLabelCloud").find('.list-label').length == 0) {
                $("#ListLabelCloud").addClass("undis");
            }
        });

        //热门标签点击
        $("#TagsCloud").on("click", "span", function () {
            if ($(this).hasClass('selected')) {
                $(this).removeClass("selected");
            } else {
                $(this).addClass("selected");
            }
            $(this).siblings().removeClass('selected');
            //更新热度
            updateHeat($(this).attr("data-id"));
            searchGroup();
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
            var item = null, itemTag = null, key = $("#GroupName_Search").val().trim().toLowerCase(), tagName = $("#TagsCloud").find("span.selected").attr("data-text");
            var _data = [];
            for (var i = 0, length = BaseData.length; i < length; i++) {
                item = BaseData[i];
                if (item.Group.GroupName.toLowerCase().indexOf(key) >= 0) {
                    if (tagName) {
                        if (item.TagList != null && item.TagList.length > 0) {
                            for (var j = 0, tLength = item.TagList.length; j < tLength; j++) {
                                itemTag = item.TagList[j];
                                if (itemTag.TagName == tagName) {
                                    _data.push(item);
                                    break;
                                }
                            }
                        }
                    } else {
                        _data.push(item);
                    }
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
            var tagList = [];
            $("#ListLabelCloud .list-label").each(function (i, item) {
                tagList.push({ TagName: $(item).text(), TagGUID: $(item).attr('data-tagid') });
            });
            var ajaxData = { Group: { GroupType: strGroupType }, ListOptions: [], TagList: tagList };
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
                    searchGroup();
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
