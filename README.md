## 概要
VRChatのシェーダーのために、次に示すメッシュを生成するUnityパッケージ
- GPUパーティクル用のpoint polygonメッシュ
- 元となるメッシュを複製して1つのShaderで扱えるメッシュ。
- Bounceが設定されたメッシュ。

## 使用法
Unity上部メニューの
Editor/MeshGenerator
より、エディタ画面が開く。

エディタ画面上、
- Num は Point Polygonの数または、複製するメッシュの数
- Rename? は 生成するメッシュの名前を設定するかどうが。特に、メッシュを複製するときに使う。
  - Rename Name はメッシュの名前を指定
- Set Bounce? は Bounceの設定
  - Bounce Size は Bounceのサイズの指定
- ボタン Generate Point Polygons は Point Polygonメッシュを生成する
- ボタン Mesh N Copy は 元となるメッシュを複製したメッシュを生成する
- ボタン Append Bounce は 元となるメッシュにBounceを加えたメッシュをを生成する

## 生成されるメッシュについて
生成されたメッシュのpoint polygonや複製されたポリゴンは、0から(指定した数-1)までのidを持つ。
このidは texcoord の z座標に指定されている。

## シェーダーのテンプレートについて
Shader Tempalte フォルダ内には、
- GPUパーティクルのテンプレートシェーダーテンプレート
- 複製したメッシュをSurfaceシェーダで扱うシェーダーテンプレート
- 複製したメッシュをUnlitシェーダで扱うシェーダーテンプレート
がある。
また、GPUパーティクルのシェーダーテンプレートと複製したメッシュをUnlitシェーダーで扱うシェーダーテンプレートでは、instanceN 定数を1から31の範囲で指定し、geometryシェーダーによってメッシュの数を指定した数倍にすることができる。

## その他
再配布禁止、使用は自己責任、著作権表記とか要らない、営利非営利問わない。
Twitter : https://twitter.com/Yumuru_n
