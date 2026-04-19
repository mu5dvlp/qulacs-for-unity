最新のプログラム仕様に合わせてテストを更新する。

## 手順

1. **現在の公開 API を把握する**
   - `Packages/com.mu5dvlp.qulacs/Runtime/QuantumState.cs`
   - `Packages/com.mu5dvlp.qulacs/Runtime/QuantumCircuit.cs`
   - `Packages/com.mu5dvlp.qulacs/Runtime/Internal/NativeMethods.cs`
   - `Packages/com.mu5dvlp.qulacs/docs/api-reference.md`（あれば）

2. **既存テストとのギャップを洗い出す**
   - `Packages/com.mu5dvlp.qulacs/Tests/QuantumStateTests.cs`
   - `Packages/com.mu5dvlp.qulacs/Tests/GateTests.cs`
   - `Packages/com.mu5dvlp.qulacs/Tests/QuantumCircuitTests.cs`
   - 各 public メソッド・プロパティについて、対応するテストケースが存在するか確認する

3. **不足しているテストを追加する**（優先順位順）

   ### QuantumStateTests.cs に追加すべきもの
   - `SetStateVector` — 任意ベクトルをセットし `GetStateVector` で往復確認
   - `GetEntropy` — |0⟩ のエントロピーが 0、等確率重ね合わせのエントロピーが正の値であることを確認
   - `Sampling(int count)`（seed なし版）— 戻り値の長さと範囲のみ検証

   ### GateTests.cs に追加すべきもの
   - `Identity` — |0⟩ に適用しても状態が変化しないことを確認
   - `SqrtX / SqrtXdag` — `SqrtX` を 2 回適用すると X と等価になることを確認
   - `SqrtY / SqrtYdag` — 同上（Y）
   - `P0 / P1` — |0⟩ に P0 を適用すると状態が保たれ、P1 を適用すると振幅がゼロになることを確認
   - `CZ` — Bell 基底での位相キックバックを確認
   - `U1 / U2 / U3` — 特定パラメータで既知ゲート（Z, H, X）と等価になることを確認
   - `Measure` — 測定後に状態が崩壊（ノルムが 0 または 1 の基底状態になる）することを確認

   ### QuantumCircuitTests.cs に追加すべきもの
   - `CalculateDepth` — 直列ゲートの深さが期待値に一致することを確認
   - `IsClifford` — H+CNOT回路は true、R(θ)を含む回路は false
   - `IsGaussian` — 適切なゲート組み合わせで true/false を確認
   - `RemoveGate` — ゲート削除後に `GateCount` が減ること、削除後の回路の動作を確認
   - `MoveGate` — ゲート順序の変更が状態更新結果に正しく反映されることを確認

4. **テスト記述のルール**
   - Qulacs の回転ゲート規約に注意: `R{X,Y,Z}(θ) = exp(+iθP/2)`（標準とは符号が逆）
   - 許容誤差は `const double Eps = 1e-6` を使用
   - 各テストは `[Test]` 属性付きの独立したメソッドとし、`using` で `QuantumState` / `QuantumCircuit` を確実に破棄する
   - テスト名は `対象_条件_期待結果` の形式にする

5. **変更後の確認**
   - 追加・変更したテストが既存テストと重複していないことを確認する
   - CLAUDE.md の Unity コマンドでテストを実行して全件パスすることを確認する:
     ```
     "C:\Program Files\Unity\Hub\Editor\6000.4.1f1\Editor\Unity.exe" \
       -projectPath . -runTests -testPlatform editmode -quit -batchmode -logFile test.log
     ```
