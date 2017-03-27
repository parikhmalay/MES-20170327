﻿/*
 Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 For licensing, see LICENSE.md or http://ckeditor.com/license
*/
CKEDITOR.dialog.add("image2", function (h) {
    function v() {
        var a = this.getValue().match(w);
        (a = !!(a && 0 !== parseInt(a[1], 10))) || alert(i["invalid" + CKEDITOR.tools.capitalize(this.id)]);
        return a
    }

    function E() {
        function a(a, c) {
            g.push(b.once(a, function (a) {
                for (var b; b = g.pop() ;) b.removeListener();
                c(a)
            }))
        }
        var b = o.createElement("img"),
            g = [];
        return function (g, c, d) {
            a("load", function () {
                c.call(d, b, b.$.width, b.$.height)
            });
            a("error", function () {
                c(null)
            });
            a("abort", function () {
                c(null)
            });
            b.setAttribute("src", g + "?" + Math.random().toString(16).substring(2))
        }
    }

    function x() {
        var a = this.getValue();
        p(!1);
        a !== s.data.src ? (y(a, function (a, b, c) {
            p(!0);
            if (!a) return j(!1);
            e.setValue(b);
            f.setValue(c);
            q = b;
            r = c;
            j(z.checkHasNaturalRatio(a))
        }), k = !0) : k ? (p(!0), e.setValue(l), f.setValue(m), k = !1) : p(!0)
    }

    function A() {
        if (c) {
            var a = this.getValue();
            if (a && (a.match(w) || j(!1), "0" !== a)) {
                var b = "width" == this.id,
                    g = l || q,
                    d = m || r,
                    a = b ? Math.round(d * (a / g)) : Math.round(g * (a / d));
                isNaN(a) || (b ? f : e).setValue(a)
            }
        }
    }

    function j(a) {
        if (d) {
            if ("boolean" == typeof a) {
                if (t) return;
                c = a
            } else if (a = e.getValue(), t = !0, (c = !c) && a) a *= m / l, isNaN(a) || f.setValue(Math.round(a));
            d[c ? "removeClass" : "addClass"]("cke_btn_unlocked");
            d.setAttribute("aria-checked", c);
            CKEDITOR.env.hc && d.getChild(0).setHtml(c ? CKEDITOR.env.ie ? "■" : "▣" : CKEDITOR.env.ie ? "□" : "▢")
        }
    }

    function p(a) {
        a = a ? "enable" : "disable";
        e[a]();
        f[a]()
    }
    var w = /(^\s*(\d+)(px)?\s*$)|^$/i,
        B = CKEDITOR.tools.getNextId(),
        C = CKEDITOR.tools.getNextId(),
        b = h.lang.image2,
        i = h.lang.common,
        F = (new CKEDITOR.template('<div><a href="javascript:void(0)" tabindex="-1" title="' + b.lockRatio + '" class="cke_btn_locked" id="{lockButtonId}" role="checkbox"><span class="cke_icon"></span><span class="cke_label">' +
            b.lockRatio + '</span></a><a href="javascript:void(0)" tabindex="-1" title="' + b.resetSize + '" class="cke_btn_reset" id="{resetButtonId}" role="button"><span class="cke_label">' + b.resetSize + "</span></a></div>")).output({
                lockButtonId: B,
                resetButtonId: C
            }),
        z = CKEDITOR.plugins.image2,
        G = z.getNatural,
        o, s, D, y, l, m, q, r, k, c, t, d, n, e, f, u, b = {
            title: b.title,
            minWidth: 250,
            minHeight: 100,
            onLoad: function () {
                o = this._.element.getDocument();
                y = E()
            },
            onShow: function () {
                s = this.widget;
                D = s.parts.image;
                k = t = c = !1;
                u = G(D);
                q = l = u.width;
                r = m =
                    u.height
            },
            contents: [{
                id: "info",
                label: b.infoTab,
                elements: [{
                    type: "vbox",
                    padding: 0,
                    children: [{
                        type: "hbox",
                        widths: ["280px", "110px"],
                        align: "right",
                        children: [{
                            id: "src",
                            type: "text",
                            label: i.url,
                            onKeyup: x,
                            onChange: x,
                            setup: function (a) {
                                this.setValue(a.data.src)
                            },
                            commit: function (a) {
                                a.setData("src", this.getValue())
                            },
                            validate: CKEDITOR.dialog.validate.notEmpty(b.urlMissing)
                        }, {
                            type: "button",
                            id: "browse",
                            style: "display:inline-block;margin-top:16px;",
                            align: "center",
                            label: h.lang.common.browseServer,
                            hidden: !0,
                            filebrowser: "info:src"
                        }]
                    }]
                }, {
                    id: "alt",
                    type: "text",
                    label: b.alt,
                    setup: function (a) {
                        this.setValue(a.data.alt)
                    },
                    commit: function (a) {
                        a.setData("alt", this.getValue())
                    }
                }, {
                    type: "hbox",
                    widths: ["25%", "25%", "50%"],
                    requiredContent: "img[width,height]",
                    children: [{
                        type: "text",
                        width: "45px",
                        id: "width",
                        label: i.width,
                        validate: v,
                        onKeyUp: A,
                        onLoad: function () {
                            e = this
                        },
                        setup: function (a) {
                            this.setValue(a.data.width)
                        },
                        commit: function (a) {
                            a.setData("width", this.getValue())
                        }
                    }, {
                        type: "text",
                        id: "height",
                        width: "45px",
                        label: i.height,
                        validate: v,
                        onKeyUp: A,
                        onLoad: function () {
                            f =
                                this
                        },
                        setup: function (a) {
                            this.setValue(a.data.height)
                        },
                        commit: function (a) {
                            a.setData("height", this.getValue())
                        }
                    }, {
                        id: "lock",
                        type: "html",
                        style: "margin-top:18px;width:40px;height:20px;",
                        onLoad: function () {
                            function a(a) {
                                a.on("mouseover", function () {
                                    this.addClass("cke_btn_over")
                                }, a);
                                a.on("mouseout", function () {
                                    this.removeClass("cke_btn_over")
                                }, a)
                            }
                            var b = this.getDialog();
                            d = o.getById(B);
                            n = o.getById(C);
                            d && (b.addFocusable(d, 4), d.on("click", function (a) {
                                j();
                                a.data && a.data.preventDefault()
                            }, this.getDialog()), a(d));
                            n && (b.addFocusable(n, 5), n.on("click", function (a) {
                                if (k) {
                                    e.setValue(q);
                                    f.setValue(r)
                                } else {
                                    e.setValue(l);
                                    f.setValue(m)
                                }
                                a.data && a.data.preventDefault()
                            }, this), a(n))
                        },
                        setup: function (a) {
                            j(a.data.lock)
                        },
                        commit: function (a) {
                            a.setData("lock", c)
                        },
                        html: F
                    }]
                }, {
                    type: "hbox",
                    id: "alignment",
                    children: [{
                        id: "align",
                        type: "radio",
                        items: [
                            ["None", "none"],
                            ["Left", "left"],
                            ["Center", "center"],
                            ["Right", "right"]
                        ],
                        label: i.align,
                        setup: function (a) {
                            this.setValue(a.data.align)
                        },
                        commit: function (a) {
                            a.setData("align", this.getValue())
                        }
                    }]
                }, {
                    id: "hasCaption",
                    type: "checkbox",
                    label: b.captioned,
                    setup: function (a) {
                        this.setValue(a.data.hasCaption)
                    },
                    commit: function (a) {
                        a.setData("hasCaption", this.getValue())
                    }
                }]
            }, {
                id: "Upload",
                hidden: !0,
                filebrowser: "uploadButton",
                label: b.uploadTab,
                elements: [{
                    type: "file",
                    id: "upload",
                    label: b.btnUpload,
                    style: "height:40px",
                    size: 38
                }, {
                    type: "fileButton",
                    id: "uploadButton",
                    filebrowser: "info:src",
                    label: b.btnUpload,
                    "for": ["Upload", "upload"]
                }]
            }]
        };
    !h.config.filebrowserImageBrowseUrl && !h.config.filebrowserBrowseUrl && (b.contents[0].elements[0].children[0] =
        b.contents[0].elements[0].children[0].children[0]);
    return b
});