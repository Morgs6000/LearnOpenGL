#version 330 core
out vec4 FragColor;

vec4 convertColorToRGBA(int r, int g, int b, int a) {
	float fr = float(r) / 255;
	float fg = float(g) / 255;
	float fb = float(b) / 255;
	float fa = float(a) / 255;

	return vec4(fr, fg, fb, fa);
}

//vec4 convertColorToHex(string hex, int a) {
//	int fr;
//	int fg;
//	int fb;
//	int fa = a / 255;
//
//	return convertColorToRGBA(fr, fg, fb, fa);
//}

void main() {
	//FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
	FragColor = convertColorToRGBA(255, 127, 51, 255);
}
