uniform sampler2D iChannel0;
uniform sampler2D iChannel1;
uniform sampler2D iChannel2;
uniform float iTime;
uniform vec2 iResolution;
uniform vec4 iMouse;
#define EPS  .01
#define COL0 vec3(.2, .35, .55)
#define COL1 vec3(.9, .43, .34)
#define COL2 vec3(.96, .66, .13)
#define COL3 vec3(0.0)
#define COL4 vec3(0.99,0.1,0.09)




/*  Install  Istructions

sudo apt-get install g++ cmake git
 sudo apt-get install libsoil-dev libglm-dev libassimp-dev libglew-dev libglfw3-dev libxinerama-dev libxcursor-dev
libxi-dev libfreetype-dev libgl1-mesa-dev xorg-dev

git clone https://github.com/JoeyDeVries/LearnOpenGL.git*/

float df_circ(in vec2 p, in vec2 c, in float r)
{
    return abs(r - length(p - c));
}

// Find the intersection of "p" onto "ab".
vec2 intersect (vec2 p, vec2 a, vec2 b)
{
    // Calculate the unit vector from "a" to "b".
    vec2 ba = normalize(b - a);

    // Calculate the intersection of p onto "ab" by
    // calculating the dot product between the unit vector
    // "ba" and the direction vector from "a" to "p", then
    // this value is multiplied by the unit vector "ab"
    // fired from the point "a".
    return a + ba * dot(ba, p - a);
}


// Visual line for debugging purposes.
bool line (vec2 p, vec2 a, vec2 b)
{
    // Direction from a to b.
    vec2 ab = normalize(b - a);

    // Direction from a to the pixel.
    vec2 ap = p - a;

    // Find the intersection of the pixel on to vector
    // from a to b, calculate the distance between the
    // pixel and the intersection point, then compare
    // that distance to the line width.
    return length((a + ab * dot(ab, ap)) - p) < 0.0025;
}

float df_line(in vec2 p, in vec2 a, in vec2 b)
{
    vec2 pa = p - a, ba = b - a;
        float h = clamp(dot(pa,ba) / dot(ba,ba), 0., 1.);
        return length(pa - ba * h);
}

float sharpen(in float d, in float w)
{
    float e = 1. / min(iResolution.y , iResolution.x);
    return 1. - smoothstep(-e, e, d - w);
}


float signedArea(in vec3 x, in vec3 y, in vec3 z)
{
    return cross(y - x, z - x).y;
}


vec3 bary(in vec3 a, in vec3 b, in vec3 c, in vec3 p)
{
    float total = signedArea(a, b, c);

    float alpha = signedArea(p, b, c) / total; // oposto de A (aresta BC)
    float beta  = signedArea(a, p, c) / total; // oposto de B (aresta CA)
    float gamma = 1.0 - alpha - beta;          // os tres sempre somam 1

    return vec3(alpha, beta, gamma);
}

bool test(in vec2 a, in vec2 b, in vec2 c, in vec2 p, inout vec3 barycoords)
{
    barycoords = bary(vec3(a.x, 0., a.y),
                  vec3(b.x, 0., b.y),
                  vec3(c.x, 0., c.y),
                  vec3(p.x, 0., p.y));

    return barycoords.x > 0. && barycoords.y > 0. && barycoords.z > 0.;
}


float df_bounds(in vec2 uv, in vec2 p, in vec2 a, in vec2 b, in vec2 c, in vec3 barycoords)
{
    float cp = 0.;





    return cp;
}

/*vec3 scene(in vec2 uv, in vec2 a, in vec2 b, in vec2 c, in vec2 p)
{
    float d = df_bounds(uv, p, a, b, c);
    return d > 0. ? COL3 : COL1;
}*/

vec3 globalColor (in vec2 uv, in vec2 a, in vec2 b, in vec2 c)
{
    vec3 colA = vec3(0.0, 1.0, 0.0);   // A: verde
    vec3 colB = vec3(1.0, 0.0, 0.0);   // B: vermelho
    vec3 colC = vec3(0.0, 0.0, 1.0);   // C: azul

    vec3 w;                            
    test(a, b, c, uv, w);

    return w.x * colA + w.y * colB + w.z * colC;
}

void main()
{
    float ar = iResolution.x / iResolution.y;
        vec2 mc=vec2(0.0);
        vec2 uv = (gl_FragCoord.xy / iResolution.xy * 2. - 1.) * vec2(ar, 1.);
            if(iMouse.z==1.0)
             mc = (iMouse.xy    / iResolution.xy * 2. - 1.) * vec2(ar, 1.);


    vec2 a = vec2( .73,  .75);
    vec2 b = vec2(-.85,  .15);
    vec2 c = vec2( .25, -.75);
    
    vec3 mouseBary;// bary do mouse
    bool mouseInside  = test(a, b, c, mc, mouseBary);
    bool mouseActive  = iMouse.z == 1.0;          // quando apertar e segurar o botão esquerdo
    bool mouseOutside = mouseActive && !mouseInside;

    
    vec3 pixelBary;
    bool pixelInside = test(a, b, c, uv, pixelBary);

    
    vec3 col = vec3(0.0);                          // background de preto
    if (mouseOutside)
        col = vec3(0.25, 0.0, 0.0);                // botão esquerdo gera vermelho esuro
    if (pixelInside)
        col = globalColor(uv, a, b, c);            // cores dentro do triangulo

   
    if (line(uv, a, b)) col = vec3(1.0, 1.0, 0.0);
    if (line(uv, b, c)) col = vec3(1.0, 0.0, 1.0);
    if (line(uv, c, a)) col = vec3(0.0, 1.0, 1.0);

    
    if (mouseOutside)
    {
        vec3  hl = vec3(1.0, 0.15, 0.15);          
        float w  = 0.008;                          
        if (mouseBary.x < 0.) col = mix(col, hl, sharpen(df_line(uv, b, c), w));
        if (mouseBary.y < 0.) col = mix(col, hl, sharpen(df_line(uv, c, a), w));
        if (mouseBary.z < 0.) col = mix(col, hl, sharpen(df_line(uv, a, b), w));
    }

    
    if (df_circ(uv, a, EPS) < 0.5*EPS) col = vec3(0.0, 1.0, 0.0);
    if (df_circ(uv, b, EPS) < 0.5*EPS) col = vec3(1.0, 0.0, 0.0);
    if (df_circ(uv, c, EPS) < 0.5*EPS) col = vec3(0.0, 0.0, 1.0);

    // ponto branco se apertar dentro do triangulo.
    if (mouseActive)
        col = mix(col, vec3(1.0), sharpen(length(uv - mc), 0.015));

    gl_FragColor = vec4(col, 1.0);
}
