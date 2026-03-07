window.memoLongPress = {
    // 一覧コンテナ内の各メモ行へ長押し判定を設定する
    initialize(container, dotNetRef) {
        if (!container) {
            return;
        }

        // 再初期化時は古いイベントを破棄する
        const existingAbortController = container._memoLongPressAbortController;
        if (existingAbortController) {
            existingAbortController.abort();
        }

        const abortController = new AbortController();
        container._memoLongPressAbortController = abortController;
        container._memoLongPressDotNetRef = dotNetRef;

        const rows = container.querySelectorAll('.memo-row[data-memo-id]');
        rows.forEach(row => {
            // 行ごとの長押し状態
            let pressTimer = null;
            let startX = 0;
            let startY = 0;
            let longPressed = false;
            let suppressClick = false;
            const moveThreshold = 8;

            // 保留中の長押しタイマーを停止する
            const clearPressTimer = () => {
                if (pressTimer !== null) {
                    window.clearTimeout(pressTimer);
                    pressTimer = null;
                }
            };

            // タッチ終了や移動時に長押し判定を取り消す
            const cancelPress = () => {
                clearPressTimer();

                // 長押し成立後の click 抑止フラグは click ハンドラ側で解除する
                if (!longPressed) {
                    suppressClick = false;
                }

                longPressed = false;
            };

            // 指を置いた位置を記録し、500ms 継続で長押し扱いにする
            const startPress = (clientX, clientY) => {
                clearPressTimer();
                startX = clientX;
                startY = clientY;
                longPressed = false;
                suppressClick = false;

                pressTimer = window.setTimeout(async () => {
                    pressTimer = null;
                    longPressed = true;
                    suppressClick = true;

                    // C# 側へ対象メモの ID を渡してコンテキストメニューを開く
                    const memoId = Number(row.dataset.memoId);
                    if (!Number.isNaN(memoId) && container._memoLongPressDotNetRef) {
                        await container._memoLongPressDotNetRef.invokeMethodAsync('OnMemoLongPress', memoId);
                    }
                }, 500);
            };

            // タッチ開始時に長押し判定を開始する
            row.addEventListener('touchstart', event => {
                if (event.touches.length !== 1) {
                    cancelPress();
                    return;
                }

                const touch = event.touches[0];
                startPress(touch.clientX, touch.clientY);
            }, { passive: true, signal: abortController.signal });

            // 指が一定以上動いたらスクロール操作とみなして長押しを取り消す
            row.addEventListener('touchmove', event => {
                if (pressTimer === null || event.touches.length !== 1) {
                    return;
                }

                const touch = event.touches[0];
                if (Math.abs(touch.clientX - startX) > moveThreshold || Math.abs(touch.clientY - startY) > moveThreshold) {
                    cancelPress();
                }
            }, { passive: true, signal: abortController.signal });

            // タッチ終了やキャンセル時は必ず判定を解除する
            row.addEventListener('touchend', cancelPress, { passive: true, signal: abortController.signal });
            row.addEventListener('touchcancel', cancelPress, { passive: true, signal: abortController.signal });
            row.addEventListener('scroll', cancelPress, { passive: true, signal: abortController.signal });

            // ブラウザ標準の長押しメニューは使わない
            row.addEventListener('contextmenu', event => {
                event.preventDefault();
            }, { signal: abortController.signal });

            // 長押し直後に click が発火した場合は通常タップを抑止する
            row.addEventListener('click', event => {
                if (!suppressClick) {
                    return;
                }

                event.preventDefault();
                event.stopPropagation();
                longPressed = false;
                suppressClick = false;
            }, { signal: abortController.signal, capture: true });
        });
    },

    // 一覧コンテナに設定したイベントを破棄する
    dispose(container) {
        if (!container) {
            return;
        }

        const abortController = container._memoLongPressAbortController;
        if (abortController) {
            abortController.abort();
            container._memoLongPressAbortController = null;
        }

        container._memoLongPressDotNetRef = null;
    }
};
