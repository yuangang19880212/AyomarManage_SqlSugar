 <template>
<ul class="nav nav-sub dk"><li v-for="item in Side.items" :class="{'active': item.id==$route.params.itemId}"><a :href="item.url+ParamUrl+item.id" class="auto"><span v-text="item.text"></span></a></li></ul>
 </template>
 <script>
 export default {
 data() {
            return {
                Side: this.$parent.Menus.filter(item => { return item.id == this.$route.params.moduleId })[0].menus.filter(item => {
                    return item.id == this.$route.params.sideId
                })[0],
                ParamUrl: '#/' + this.$route.params.moduleId + '/' + this.$route.params.sideId + "/"
            }
        },
        watch: {
            '$route'(to, from) {
                this.Side = this.$parent.Menus.filter(item => {
                    return item.id == this.$route.params.moduleId
                })[0].menus.filter(item => {
                    return item.id == this.$route.params.sideId
                })[0];
                this.ParamUrl = '#/' + this.$route.params.moduleId + '/' + this.$route.params.sideId + "/"
            }
        }
 }
 </script>
 