import Vue from 'vue'
import VueRouter from 'vue-router'

Vue.use(VueRouter);

const DEFAULT_TITLE = process.env.VUE_APP_DEFAULT_TITLE;

const routes = [
    {
      path: '/',
      name: 'Menu',
      redirect: '/login',
      component: () => import('@/views/routerhandler.vue'),  
      children: [
        {
          path: 'login',
          name: 'Login',
          component: () => import('@/views/home.vue')
        },
        {
          path: 'mainscreen',
          name: 'main',
          props: true,
          component: () => import('@/views/restcalls.vue')
          
        }
      ]
    },
]


const router = new VueRouter({
    mode: 'history',
    base: process.env.BASE_URL,
    routes,
    scrollBehavior(to, from, savedPosition) {
        if (savedPosition) {
            return savedPosition
        } else {
            return { x: 0, y: 0 }
        }
    }
});

router.afterEach((to) => {
    // Use next tick to handle router history correctly
    // see: https://github.com/vuejs/vue-router/issues/914#issuecomment-384477609
    Vue.nextTick(() => {
        //console.log('DEFAULT_TITLE', DEFAULT_TITLE);
        document.title = to.meta.title ? `${to.meta.title} - ${DEFAULT_TITLE}` : DEFAULT_TITLE;
    });
});



export default router