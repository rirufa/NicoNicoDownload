# NicoNicoDownload
ニコニコ動画からダウンロードするツール

#使い方
1.downloadフォルダーを作る
2.list.txtにニコニコ動画のIDを書く
3.format.txtを書く
4.exeをダブルクリックする

# format.txtの書式
1行目は変換先のタイトルを書く。%title%はタイトルを表す。
2行目以降には変換元のタイトルを書く。正規表現が使用可能。タイトルに相当する部分は(?<title>.+)という形に書く必要があります。

## 注意点
ファイル名として使用できない文字はすべて全角に変換しています。
使用可能な正規表現は.NET Frameworkと同じです

## 例
東方ヴォーカル___%title%
(?<title>.+)【.+】.+
\[.+\](?<title>.+)

# コマンドラインパラメーター
NicoNicoDownloader [ID] [PASSWORD]

#権利
NicoNico.NetとNicoNico.Net.ConsoleTestの権利はdrasticactionsに帰属します