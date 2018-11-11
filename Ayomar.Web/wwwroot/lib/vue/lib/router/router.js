/**
 *  路由文件
 *  引入[Vue, VueRouter]模块
 */
import Vue from 'vue'
import VueRouter from 'vue-router'
/**
 *  加载模块
 */
Vue.use(VueRouter)
/**
 *  配置路由
 */
const Items = {}
const routers = new VueRouter({
    routes: [
        {
            path: '/:moduleId', name: 'modules', redirect: '/:moduleId/:side', component: function (resolve) { require(['/lib/vue/lib/router/Modules.vue'], resolve)}, children: [
                {
                    path: ':sideId', name: 'sides', component: function (resolve) { require(['/lib/vue/lib/router/Sides.vue'], resolve) }, children: [
                        { path: ':itemId', name: 'items', component: Items }
                    ]
                }
            ]
        }
    ]
})
export default router