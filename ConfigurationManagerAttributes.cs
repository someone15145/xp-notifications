namespace XPNotifications
{
    /// <summary>
    /// Класс, определяющий, как настройка должна отображаться в окне настроек ConfigurationManager.
    /// 
    /// Использование:
    /// Этот шаблон класса необходимо скопировать в проект плагина и ссылаться на него напрямую из кода.
    /// Создайте новый экземпляр, назначьте любые поля, которые вы хотите переопределить, и передайте его в качестве тега (tag) для вашей настройки.
    /// 
    /// Если поле равно null (по умолчанию), оно будет проигнорировано и не изменит отображение настройки.
    /// Если полю присвоено значение, оно переопределит поведение по умолчанию.
    /// </summary>
    /// 
    /// <example> 
    /// Пример переопределения порядка настроек и пометка одной из них как расширенной:
    /// <code>
    /// // Переопределяем IsAdvanced и Order
    /// Config.Bind("X", "1", 1, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true, Order = 3 }));
    /// // Переопределяем только Order, IsAdvanced остается со значением по умолчанию от ConfigManager
    /// Config.Bind("X", "2", 2, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
    /// Config.Bind("X", "3", 3, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 2 }));
    /// </code>
    /// </example>
    /// 
    /// <remarks> 
    /// Подробнее и с примерами можно ознакомиться в readme по адресу https://github.com/BepInEx/BepInEx.ConfigurationManager
    /// Вы можете по желанию удалить неиспользуемые поля из этого класса — это равносильно тому, чтобы оставить их null.
    /// </remarks>
#pragma warning disable 0169, 0414, 0649
    internal sealed class ConfigurationManagerAttributes
    {
        /// <summary>
        /// Должна ли настройка отображаться как процент (используйте только для настроек с диапазоном значений).
        /// </summary>
        public bool? ShowRangeAsPercent;

        /// <summary>
        /// Пользовательский редактор настройки (код OnGUI, который заменяет стандартный редактор ConfigurationManager).
        /// См. ниже для подробного объяснения. Использование пользовательского отрисовщика (CustomDrawer) 
        /// приведет к тому, что многие другие поля перестанут работать.
        /// </summary>
        public System.Action<BepInEx.Configuration.ConfigEntryBase> CustomDrawer;

        /// <summary>
        /// Пользовательский редактор, позволяющий опрашивать ввод с клавиатуры с помощью класса Input (или UnityInput).
        /// Используйте либо CustomDrawer, либо CustomHotkeyDrawer; использование обоих одновременно приведет к неопределенному поведению.
        /// </summary>
        public CustomHotkeyDrawerFunc CustomHotkeyDrawer;

        /// <summary>
        /// Функция отрисовки, позволяющая опрашивать ввод с клавиатуры.
        /// Примечание: Обязательно делайте фокус на вашем элементе управления UI, когда принимаете ввод, 
        /// чтобы пользователь не печатал в поле поиска или в другой настройке (лучше всего делать это в каждом кадре).
        /// Если вы не отрисовываете выбираемые элементы UI, можно использовать `GUIUtility.keyboardControl = -1;` 
        /// в каждом кадре, чтобы убедиться, что ничего не выбрано.
        /// </summary>
        /// <example>
        /// CustomHotkeyDrawer = (ConfigEntryBase setting, ref bool isEditing) =>
        /// {
        ///     if (isEditing)
        ///     {
        ///         // Убеждаемся, что ничего не выбрано, так как мы не фокусируемся на текстовом поле через GUI.FocusControl.
        ///         GUIUtility.keyboardControl = -1;
        ///                     
        ///         // Используйте Input.GetKeyDown и прочее здесь, не забудьте установить isEditing в false после завершения!
        ///         // Рекомендуется проверять Input.anyKeyDown и сразу ставить isEditing = false,
        ///         // чтобы ввод не успел передаться в саму игру.
        /// 
        ///         if (GUILayout.Button("Stop"))
        ///             isEditing = false;
        ///     }
        ///     else
        ///     {
        ///         if (GUILayout.Button("Start"))
        ///             isEditing = true;
        ///     }
        /// 
        ///     // Будет true, только когда isEditing == true и зажата любая клавиша
        ///     GUILayout.Label("Any key pressed: " + Input.anyKey);
        /// }
        /// </example>
        /// <param name="setting">
        /// Текущая настраиваемая установка (если доступна).
        /// </param>
        /// <param name="isCurrentlyAcceptingInput">
        /// Установите этот ref-параметр в true, если хотите, чтобы текущий отрисовщик получал события ввода (Input).
        /// Значение сохраняется после установки; используйте его, чтобы проверить, редактируется ли данный экземпляр.
        /// Не забудьте вернуть false после завершения!
        /// </param>
        public delegate void CustomHotkeyDrawerFunc(BepInEx.Configuration.ConfigEntryBase setting, ref bool isCurrentlyAcceptingInput);

        /// <summary>
        /// Показывать ли эту настройку в окне настроек вообще? Если false — скрыть.
        /// </summary>
        public bool? Browsable;

        /// <summary>
        /// Категория, в которой находится настройка. Null — настройка будет находиться прямо в разделе плагина.
        /// </summary>
        public string Category;

        /// <summary>
        /// Если установлено, рядом с настройкой появится кнопка "Default" (По умолчанию) для сброса значения.
        /// </summary>
        public object DefaultValue;

        /// <summary>
        /// Принудительно скрыть кнопку сброса ("Reset"), даже если указано валидное DefaultValue.
        /// </summary>
        public bool? HideDefaultButton;

        /// <summary>
        /// Принудительно скрыть название настройки. Рекомендуется использовать только вместе с 
        /// <see cref="CustomDrawer"/>, чтобы получить больше свободного места.
        /// Можно комбинировать с <see cref="HideDefaultButton"/> для максимальной экономии места.
        /// </summary>
        public bool? HideSettingName;

        /// <summary>
        /// Опциональное описание, отображаемое при наведении на настройку.
        /// Не рекомендуется — лучше указывать описание сразу при создании настройки.
        /// </summary>
        public string Description;

        /// <summary>
        /// Отображаемое имя настройки.
        /// </summary>
        public string DispName;

        /// <summary>
        /// Порядок настройки в списке относительно других настроек в той же категории.
        /// По умолчанию 0. Чем выше число, тем выше настройка в списке.
        /// </summary>
        public int? Order;

        /// <summary>
        /// Только для чтения: показывать значение, но запретить его редактирование.
        /// </summary>
        public bool? ReadOnly;

        /// <summary>
        /// Если true, настройка не будет отображаться по умолчанию. 
        /// Пользователю придется включить "Advanced settings" или найти её через поиск.
        /// </summary>
        public bool? IsAdvanced;

        /// <summary>
        /// Пользовательский конвертер из типа настройки в строку для стандартных текстовых полей редактора.
        /// </summary>
        public System.Func<object, string> ObjToStr;

        /// <summary>
        /// Пользовательский конвертер из строки в тип настройки для стандартных текстовых полей редактора.
        /// </summary>
        public System.Func<string, object> StrToObj;
    }
}
