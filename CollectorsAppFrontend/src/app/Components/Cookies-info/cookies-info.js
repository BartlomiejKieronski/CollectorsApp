import "./cookies-info.module.css";
import cn from "classnames";

export default function CooiesInfo() {

    return (
        <div className={cn(Style.cookieNoticeItemStyle)}>
            <div className={cn(Style.displayedData)}>
                <div>
                    message
                </div>
                <div>
                    <button>
                        Ok
                    </button>
                </div>
            </div>

        </div>)
}