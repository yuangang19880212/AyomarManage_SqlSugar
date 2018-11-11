 <template>
 <nav class="navi">
 <ul class="nav" v-for="module in Menus">
       <li class="hidden-folded padder m-t m-b-sm text-muted text-xs" >
       <span v-text="module.text"></span>
       <b class="label bg-primary pull-right" v-text="module.menus.length"></b>
       </li>
       <li v-for="side in module.menus" :class="{'active': side.id==$route.params.sideId}" >
       <router-link :to="{name:\'sides\',params: { sideId: side.id,item:side.items[0].id }}" class="auto">
       <span class="pull-right text-muted">
       <i class="fa fa-fw fa-angle-right text"></i>
       <i class="fa fa-fw fa-angle-down text-active"></i>
       </span>
       <i :class="side.icon"></i>
       <span v-text="side.text"></span>
       </router-link><router-view></router-view>
       </li>
       <li class="line dk hidden-folded"></li>
       </ul><ul class="nav">
       <li class="hidden-folded padder m-t m-b-sm text-muted text-xs" >
       <span>Your Stuff</span>
       </li>
       <li>
        <a ui-sref="app.page.profile"> 
       <i class="icon-user icon text-success-lter" ></i> 
       <b class="badge bg-success pull-right" > 30 %</b> 
       <span> Profile</span> 
       </a> 
       </li> 
       <li> 
       <a href= "javascript:void(0)"> 
       <i class="fa fa-question-circle-o" ></i > 
       <span> 联系我们</span> 
       </a> 
       </li> 
       </ul ></nav>
 </template>
 <script>
 export default {
 data() {
            return { Menus: this.$parent.Menus.filter(item => { return item.id == this.$route.params.moduleId }) }
        }, watch: {
            '$route'(to, from) {
                this.Menus = this.$parent.Menus.filter(item => { return item.id == this.$route.params.moduleId });
                console.log(this.$parent.Menus);
            }
        }
 }
 </script>
 